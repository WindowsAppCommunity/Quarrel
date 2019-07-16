using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;

namespace Quarrel.Services.Guild
{
    public class GuildsService : IGuildsService
    {
        public Dictionary<string, BindableChannel> CurrentChannels { get; } = new Dictionary<string, BindableChannel>();
        public Dictionary<string, BindableGuild> Guilds { get; } = new Dictionary<string, BindableGuild>();
        public string CurrentGuildId { get; private set; }
        public BindableGuild CurrentGuild => Guilds[CurrentGuildId];

        private ICacheService CacheService;

        public GuildsService(ICacheService cacheService)
        {
            CacheService = cacheService;
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    #region Settings

                    foreach (var gSettings in m.EventData.GuildSettings)
                    {
                        CacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, gSettings, gSettings.GuildId);

                        foreach (var cSettings in gSettings.ChannelOverrides)
                        {
                            CacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, cSettings, cSettings.ChannelId);
                        }
                    }
                    #endregion

                    #region SortReadStates

                    Dictionary<string, ReadState> readStates = new Dictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    #endregion

                    #region Guilds and Channels

                    List<BindableGuild> guildList = new List<BindableGuild>();

                    // Add DM
                    var dmGuild = new BindableGuild(new DiscordAPI.Models.Guild() { Name = "DM", Id = "DM" });
                    guildList.Add(dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel);

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            CurrentChannels.Add(bChannel.Model.Id, bChannel);
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = new ObservableCollection<BindableChannel>(dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList());
                    }


                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        // Handle guild settings
                        GuildSetting gSettings = CacheService.Runtime.TryGetValue<GuildSetting>(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, guild.Id);
                        if (gSettings != null)
                        {
                            bGuild.Muted = gSettings.Muted;
                        }

                        // Guild Order
                        bGuild.Position = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                        // Guild Channels
                        foreach (var channel in guild.Channels)
                        {
                            IEnumerable<VoiceState> state = guild.VoiceStates?.Where(x => x.ChannelId == channel.Id);
                            BindableChannel bChannel = new BindableChannel(channel, state);
                            bChannel.GuildId = guild.Id;
                            // Handle channel settings
                            ChannelOverride cSettings = CacheService.Runtime.TryGetValue<ChannelOverride>(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, channel.Id);
                            if (cSettings != null)
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            // Find parent position
                            if (!string.IsNullOrEmpty(bChannel.ParentId))
                                bChannel.ParentPostion = guild.Channels.First(x => x.Id == bChannel.ParentId).Position;
                            else
                                bChannel.ParentPostion = -1;

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            bGuild.Channels.Add(bChannel);
                            CurrentChannels.Add(bChannel.Model.Id, bChannel);
                        }

                        bGuild.Channels = new ObservableCollection<BindableChannel>(bGuild.Channels.OrderBy(x => x.AbsolutePostion).ToList());

                        bGuild.Model.Channels = null;


                        //foreach (var user in guild.Members)
                        //{
                        //    BindableUser bgMember = new BindableUser(user);
                        //    bgMember.GuildId = guild.Id;
                        //    // TODO: Add to Memberlist
                        //}

                        // Guild Roles
                        foreach (var role in guild.Roles)
                        {
                            CacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                        }

                        // Guild Presences
                        foreach (var presence in guild.Presences)
                        {
                            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
                        }

                        guildList.Add(bGuild);
                    }

                    guildList = guildList.OrderBy(x => x.Position).ToList();
                    #endregion
                    Messenger.Default.Send("GuildsReady");

                    foreach (var guild in guildList)
                    {
                        Guilds.Add(guild.Model.Id, guild);
                    }
                });
            });
            Messenger.Default.Register<GatewayGuildChannelCreatedMessage>(this, async m =>
            {
                var bChannel = new BindableChannel(m.Channel) { GuildId = m.Channel.GuildId };
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    bChannel.ParentPostion = CurrentChannels[bChannel.ParentId].Position;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                for (int i = 0; i < Guilds[m.Channel.GuildId].Channels.Count; i++)
                {
                    if (Guilds[m.Channel.GuildId].Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                    {
                        _ = DispatcherHelper.RunAsync(()=>{
                            Guilds[m.Channel.GuildId].Channels.Insert(i, bChannel);
                        });
                        break;
                    }
                }
            });
            Messenger.Default.Register<GatewayGuildChannelDeletedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    Guilds[m.Channel.GuildId].Channels.Remove(CurrentChannels[m.Channel.Id]);
                    CurrentChannels.Remove(m.Channel.Id);
                });
            });
            Messenger.Default.Register<GuildNavigateMessage>(this, m => { CurrentGuildId = m.Guild.Model.Id; });
        }

        public BindableChannel GetChannel(string channelId)
        {
            return CurrentChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }
    }
}
