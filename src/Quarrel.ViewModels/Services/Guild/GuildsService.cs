using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.ViewModels.Services.Users;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Services.Guild
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

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    #region SortReadStates

                    IDictionary<string, ReadState> readStates = new ConcurrentDictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    #endregion

                    #region Guilds and Channels

                    // Add DM
                    var dmGuild =
                        new BindableGuild(new DiscordAPI.Models.Guild() {Name = "DM", Id = "DM"}) {Position = -1};
                    Guilds.Add(dmGuild.Model.Id, dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel, "DM");

                            ChannelOverride cSettings;
                            if (SimpleIoc.Default.GetInstance<ICurrentUsersService>().ChannelSettings.TryGetValue(channel.Id, out cSettings))
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

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
                        GuildSetting gSettings;
                        if (SimpleIoc.Default.GetInstance<ICurrentUsersService>().GuildSettings.TryGetValue(guild.Id, out gSettings))
                        {
                            bGuild.Muted = gSettings.Muted;
                        }

                        // Guild Order
                        bGuild.Position = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                        //This is needed to fix ordering when multiple categories have the same position
                        var categories = guild.Channels.Where(x => x.Type == 4).ToList();

                        foreach (var group in categories.GroupBy(x => x.Position).OrderBy(x => x.Key))
                        {
                            foreach (var channel in group.Skip(1))
                            {
                                bool shouldDo = false;
                                foreach (var category in categories)
                                {
                                    if (category.Id == channel.Id) shouldDo = true;
                                    if (shouldDo) category.Position += 1;
                                }
                            }
                            
                        }
                        
                        // Guild Channels
                        foreach (var channel in guild.Channels)
                        {
                            IEnumerable<VoiceState> state = guild.VoiceStates?.Where(x => x.ChannelId == channel.Id);
                            BindableChannel bChannel = new BindableChannel(channel, guild.Id, state);
                            // Handle channel settings
                            ChannelOverride cSettings;
                            if (SimpleIoc.Default.GetInstance<ICurrentUsersService>().ChannelSettings.TryGetValue(channel.Id, out cSettings))
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            // Find parent position
                            if (!string.IsNullOrEmpty(bChannel.ParentId) && bChannel.ParentId != bChannel.Model.Id)
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
                            CacheService.Runtime.SetValue(Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                        }

                        // Guild Presences
                        foreach (var presence in guild.Presences)
                        {
                            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
                        }

                        Guilds.Add(bGuild.Model.Id, bGuild);
                    }

                    #endregion

                    Messenger.Default.Send("GuildsReady");
                });
            });
            Messenger.Default.Register<GatewayChannelCreatedMessage>(this, async m =>
            {
                string guildId = "DM";
                if (m.Channel is GuildChannel gChannel)
                    guildId = gChannel.GuildId;

                var bChannel = new BindableChannel(m.Channel, guildId);
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    bChannel.ParentPostion = CurrentChannels.TryGetValue(bChannel.ParentId, out var value) ? value.Position : 0;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                if (Guilds.TryGetValue(guildId, out var guild))
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

                CurrentChannels.Add(bChannel.Model.Id, bChannel);
            });
            Messenger.Default.Register<GatewayChannelDeletedMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (CurrentChannels.TryGetValue(m.Channel.Id, out var currentChannel))
                    {
                        if (Guilds.TryGetValue(currentChannel.GuildId, out var value))
                        {
                            value.Channels.Remove(currentChannel);
                        }

                        CurrentChannels.Remove(m.Channel.Id);
                    }
                });
            });
            Messenger.Default.Register<GatewayGuildChannelUpdatedMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    var bChannel = GetChannel(m.Channel.Id ?? "DM");
                    bChannel.Model = m.Channel;
                    
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
                        guild.Channels.Remove(bChannel);
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
            });
            Messenger.Default.Register<GuildNavigateMessage>(this, m => { CurrentGuildId = m.Guild.Model.Id; });
        }

        public BindableChannel GetChannel(string channelId)
        {
            return CurrentChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }

        private void RemoveChannel(Channel channel)
        {
        }
    }
}
