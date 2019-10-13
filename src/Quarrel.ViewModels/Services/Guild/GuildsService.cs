using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;
using Quarrel.ViewModels.Services.DispatcherHelper;

namespace Quarrel.Services.Guild
{
    public class GuildsService : IGuildsService
    {
        public IDictionary<string, BindableChannel> CurrentChannels { get; } = new ConcurrentDictionary<string, BindableChannel>();
        public IDictionary<string, BindableGuild> Guilds { get; } = new ConcurrentDictionary<string, BindableGuild>();
        public string CurrentGuildId { get; private set; }
        public BindableGuild CurrentGuild => Guilds.TryGetValue(CurrentGuildId, out var value) ? value : null;

        private ICacheService CacheService;
        private IDispatcherHelper DispatcherHelper;

        public GuildsService(ICacheService cacheService, IDispatcherHelper dispatcherHelper)
        {
            CacheService = cacheService;
            DispatcherHelper = dispatcherHelper;
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
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

                    IDictionary<string, ReadState> readStates = new ConcurrentDictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    #endregion

                    #region Guilds and Channels

                    // Add DM
                    Guilds.Clear();

                    var dmGuild = new BindableGuild(new DiscordAPI.Models.Guild() { Name = "DM", Id = "DM" });
                    dmGuild.Position = -1;
                    Guilds.Add(dmGuild.Model.Id, dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel);

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            CurrentChannels.AddOrUpdate(bChannel.Model.Id, bChannel);
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = new ObservableCollection<BindableChannel>(dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList());
                    }


                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        Guilds.Add(bGuild.Model.Id, bGuild);

                        // Handle guild settings
                        GuildSetting gSettings = CacheService.Runtime.TryGetValue<GuildSetting>(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, guild.Id);
                        if (gSettings != null)
                        {
                            bGuild.Muted = gSettings.Muted;
                        }

                        // Guild Order
                        var order = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);
                        if (order > -1) bGuild.Position = order;

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
                            CurrentChannels.AddOrUpdate(bChannel.Model.Id, bChannel);
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
                    }

                    #endregion
                    Messenger.Default.Send("GuildsReady");
                });
            });
            Messenger.Default.Register<GatewayGuildChannelCreatedMessage>(this, async m =>
            {
                var bChannel = new BindableChannel(m.Channel) { GuildId = m.Channel.GuildId };
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    bChannel.ParentPostion = CurrentChannels.TryGetValue(bChannel.ParentId, out var value) ? value.Position : 0;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                if (Guilds.TryGetValue(m.Channel.GuildId, out var guild))
                {
                    for (int i = 0; i < guild.Channels.Count; i++)
                    {
                        if (guild.Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUi(() =>
                            {
                                guild.Channels.Insert(i, bChannel);
                            });
                            break;
                        }
                    }
                }
            });
            Messenger.Default.Register<GatewayGuildChannelDeletedMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (CurrentChannels.TryGetValue(m.Channel.Id, out var currentChannel))
                    {
                        (Guilds.TryGetValue(m.Channel.GuildId, out var value) ? value : null)?
                            .Channels.Remove(currentChannel);

                        CurrentChannels.Remove(m.Channel.Id);
                    }
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
