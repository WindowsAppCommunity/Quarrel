using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Services.Discord.Guilds
{
    public class GuildsService : IGuildsService
    {
        public ConcurrentDictionary<string, GuildSetting> GuildSettings { get; } =
            new ConcurrentDictionary<string, GuildSetting>();

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, BindableGuildMember>> _GuildUsers =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, BindableGuildMember>>();

        public IDictionary<string, BindableGuild> AllGuilds { get; } = new ConcurrentDictionary<string, BindableGuild>();
        public BindableGuild CurrentGuild { get; set; }

        private ICacheService CacheService;
        private IChannelsService ChannelsService;
        private IPresenceService PresenceService;
        private IDispatcherHelper DispatcherHelper;

        public GuildsService(ICacheService cacheService, IPresenceService presenceService, IDispatcherHelper dispatcherHelper, IChannelsService channelService)
        {
            CacheService = cacheService;
            ChannelsService = channelService;
            PresenceService = presenceService;
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
                        new BindableGuild(new Guild() {Name = "DM", Id = "DM"}) {Position = -1};
                    AllGuilds.Add(dmGuild.Model.Id, dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel, "DM");

                            ChannelOverride cSettings;
                            if (SimpleIoc.Default.GetInstance<IChannelsService>().ChannelSettings.TryGetValue(channel.Id, out cSettings))
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                                bChannel.ReadState = readStates[bChannel.Model.Id];

                            ChannelsService.AllChannels.Add(bChannel.Model.Id, bChannel);
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = new ObservableCollection<BindableChannel>(dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList());
                    }


                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        // Handle guild settings
                        GuildSetting gSettings;
                        if (GuildSettings.TryGetValue(guild.Id, out gSettings))
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
                            if (SimpleIoc.Default.GetInstance<IChannelsService>().ChannelSettings.TryGetValue(channel.Id, out cSettings))
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
                            ChannelsService.AllChannels.Add(bChannel.Model.Id, bChannel);
                        }

                        bGuild.Channels = new ObservableCollection<BindableChannel>(bGuild.Channels.OrderBy(x => x.AbsolutePostion).ToList());

                        bGuild.Model.Channels = null;

                        _GuildUsers.TryAdd(guild.Id, new ConcurrentDictionary<string, BindableGuildMember>());
                        foreach (var user in guild.Members)
                        {
                            BindableGuildMember bgMember = new BindableGuildMember(user);
                            bgMember.GuildId = guild.Id;
                            _GuildUsers[guild.Id].TryAdd(bgMember.Model.User.Id, bgMember);
                        }

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

                        AllGuilds.Add(bGuild.Model.Id, bGuild);
                    }

                    #endregion

                    Messenger.Default.Send("GuildsReady");
                });
            });
            Messenger.Default.Register<GatewayChannelCreatedMessage>(this, m =>
            {
                string guildId = "DM";
                if (m.Channel is GuildChannel gChannel)
                    guildId = gChannel.GuildId;

                var bChannel = new BindableChannel(m.Channel, guildId);
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    bChannel.ParentPostion = ChannelsService.AllChannels.TryGetValue(bChannel.ParentId, out var value) ? value.Position : 0;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                if (AllGuilds.TryGetValue(guildId, out var guild))
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

                ChannelsService.AllChannels.Add(bChannel.Model.Id, bChannel);
            });
            Messenger.Default.Register<GatewayChannelDeletedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (ChannelsService.AllChannels.TryGetValue(m.Channel.Id, out var currentChannel))
                    {
                        if (AllGuilds.TryGetValue(currentChannel.GuildId, out var value))
                        {
                            value.Channels.Remove(currentChannel);
                        }

                        ChannelsService.AllChannels.Remove(m.Channel.Id);
                    }
                });
            });
            Messenger.Default.Register<GatewayGuildChannelUpdatedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    var bChannel = ChannelsService.GetChannel(m.Channel.Id ?? "DM");
                    bChannel.Model = m.Channel;
                    
                    if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                    {
                        bChannel.ParentPostion = ChannelsService.AllChannels.TryGetValue(bChannel.ParentId, out var value) ? value.Position : 0;
                    }
                    else if (bChannel.ParentId == null)
                    {
                        bChannel.ParentPostion = -1;
                    }
                    
                    if (AllGuilds.TryGetValue(m.Channel.GuildId, out var guild))
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
            Messenger.Default.Register<GuildNavigateMessage>(this, m => { CurrentGuild = m.Guild; });
            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                _GuildUsers.TryGetValue(m.GuildMembersChunk.GuildId, out var guild);
                foreach (var member in m.GuildMembersChunk.Members)
                {
                    guild.TryAdd(member.User.Id, new BindableGuildMember(member));
                }

                if (m.GuildMembersChunk.Presences != null)
                    foreach (var presence in m.GuildMembersChunk.Presences)
                    {
                        PresenceService.UpdateUserPrecense(presence.User.Id, presence);
                    }
            });
        }

        public BindableGuildMember GetGuildMember(string memberId, string guildId)
        {
            if (_GuildUsers.TryGetValue(guildId, out var guild) && guild.TryGetValue(memberId, out BindableGuildMember member))
            {
                return member;
            }
            else
            {
                return null;
            }
        }
        public IReadOnlyDictionary<string, BindableGuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId)
        {
            Dictionary<string, BindableGuildMember> guildMembers = new Dictionary<string, BindableGuildMember>();
            List<string> guildMembersToBeRequested = new List<string>();
            if (_GuildUsers.TryGetValue(guildId, out var guild))
            {
                foreach (string memberId in memberIds)
                {
                    if (guild.TryGetValue(memberId, out BindableGuildMember member))
                    {
                        guildMembers.Add(memberId, member);
                    }
                    else
                    {
                        guildMembersToBeRequested.Add(memberId);
                    }
                }

                if (guildMembersToBeRequested.Count > 0)
                {
                    Messenger.Default.Send(new GatewayRequestGuildMembersMessage(new List<string> { guildId }, guildMembersToBeRequested));
                }

                return guildMembers;
            }
            return null;
        }
    }
}
