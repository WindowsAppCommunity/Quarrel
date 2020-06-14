// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Voice;
using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Voice;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IVoiceService = Quarrel.ViewModels.Services.Voice.IVoiceService;

namespace Quarrel.ViewModels.Services.Discord.Guilds
{
    /// <summary>
    /// Manages all guild information.
    /// </summary>
    public class GuildsService : IGuildsService
    {
        private readonly IDictionary<string, ConcurrentDictionary<string, BindableGuildMember>> _guildUsers =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, BindableGuildMember>>();

        private readonly IAnalyticsService _analyticsService;
        private readonly ICacheService _cacheService;
        private readonly IChannelsService _channelsService;
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IPresenceService _presenceService;
        private IVoiceService _voiceService;
        private MainViewModel _mainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildsService"/> class.
        /// </summary>
        /// <param name="analyticsService">The app's analytics service.</param>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="channelsService">The app's channel service.</param>
        /// <param name="presenceService">The app's presence service.</param>
        /// <param name="dispatcherHelper">The app's dispatcher helper.</param>
        public GuildsService(
            IAnalyticsService analyticsService,
            ICacheService cacheService,
            IChannelsService channelsService,
            IDispatcherHelper dispatcherHelper,
            IPresenceService presenceService)
        {
            _analyticsService = analyticsService;
            _cacheService = cacheService;
            _channelsService = channelsService;
            _dispatcherHelper = dispatcherHelper;
            _presenceService = presenceService;

            // _voiceService = voiceService;
            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    IDictionary<string, ReadState> readStates = new ConcurrentDictionary<string, ReadState>();
                    foreach (var state in m.EventData.ReadStates)
                    {
                        readStates.Add(state.Id, state);
                    }

                    // Add DM
                    var dmGuild =
                        new BindableGuild(new Guild() { Name = "DM", Id = "DM" }) { Position = -1 };

                    AddOrUpdateGuild(dmGuild.Model.Id, dmGuild);

                    // Add DM channels
                    if (m.EventData.PrivateChannels != null && m.EventData.PrivateChannels.Any())
                    {
                        foreach (var channel in m.EventData.PrivateChannels)
                        {
                            BindableChannel bChannel = new BindableChannel(channel);

                            ChannelOverride cSettings = _channelsService.GetChannelSettings(channel.Id);
                            if (cSettings != null)
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            dmGuild.Channels.Add(bChannel);

                            if (readStates.ContainsKey(bChannel.Model.Id))
                            {
                                bChannel.ReadState = readStates[bChannel.Model.Id];
                                if (bChannel.ReadState.LastMessageId != bChannel.Model.LastMessageId)
                                {
                                    bChannel.ReadState.MentionCount++;
                                }
                            }

                            _channelsService.AddOrUpdateChannel(bChannel.Model.Id, bChannel);
                        }

                        // Sort by last message timestamp
                        dmGuild.Channels = new ObservableCollection<BindableChannel>(dmGuild.Channels.OrderByDescending(x => Convert.ToUInt64(x.Model.LastMessageId)).ToList());
                    }

                    foreach (var guild in m.EventData.Guilds)
                    {
                        BindableGuild bGuild = new BindableGuild(guild);

                        // Handle guild settings
                        GuildSetting gSettings = GetGuildSetting(guild.Id);
                        if (gSettings != null)
                        {
                            bGuild.IsMuted = gSettings.Muted;
                        }

                        // Guild Order
                        bGuild.Position = m.EventData.Settings.GuildOrder.IndexOf(x => x == bGuild.Model.Id);

                        if (guild.Unavailable)
                        {
                            // TODO:
                            // This should be updated to display information about unavailable guild
                            // We also need a mechanism to readd the guild once it becomes available
                            continue;
                        }

                        // This is needed to fix ordering when multiple categories have the same position
                        var categories = guild.Channels.Where(x => x.Type == 4).ToList();

                        foreach (var group in categories.GroupBy(x => x.Position).OrderBy(x => x.Key))
                        {
                            foreach (var channel in group.Skip(1))
                            {
                                bool shouldDo = false;
                                foreach (var category in categories)
                                {
                                    if (category.Id == channel.Id)
                                    {
                                        shouldDo = true;
                                    }

                                    if (shouldDo)
                                    {
                                        category.Position += 1;
                                    }
                                }
                            }
                        }

                        if (guild.VoiceStates != null)
                        {
                            foreach (var state in guild.VoiceStates)
                            {
                                VoiceService.VoiceStates[state.UserId] = state;
                            }
                        }

                        // Guild Channels
                        foreach (var channel in guild.Channels)
                        {
                            channel.GuildId = guild.Id;
                            IEnumerable<VoiceState> state = guild.VoiceStates?.Where(x => x.ChannelId == channel.Id);
                            BindableChannel bChannel = new BindableChannel(channel, state);

                            // Handle channel settings
                            ChannelOverride cSettings = _channelsService.GetChannelSettings(channel.Id);
                            if (cSettings != null)
                            {
                                bChannel.Muted = cSettings.Muted;
                            }

                            // Find parent position
                            if (!string.IsNullOrEmpty(bChannel.ParentId) && bChannel.ParentId != bChannel.Model.Id)
                            {
                                bChannel.ParentPostion = guild.Channels.First(x => x.Id == bChannel.ParentId).Position;
                            }
                            else
                            {
                                bChannel.ParentPostion = -1;
                            }

                            if (readStates.ContainsKey(bChannel.Model.Id))
                            {
                                bChannel.ReadState = readStates[bChannel.Model.Id];
                            }

                            bGuild.Channels.Add(bChannel);
                            _channelsService.AddOrUpdateChannel(bChannel.Model.Id, bChannel);
                        }

                        bGuild.Channels = new ObservableCollection<BindableChannel>(bGuild.Channels.OrderBy(x => x.AbsolutePostion).ToList());

                        bGuild.Model.Channels = null;

                        _guildUsers.AddOrUpdate(guild.Id, new ConcurrentDictionary<string, BindableGuildMember>());
                        foreach (var user in guild.Members)
                        {
                            BindableGuildMember bgMember = new BindableGuildMember(user, guild.Id);
                            _guildUsers[guild.Id].TryAdd(bgMember.Model.User.Id, bgMember);
                        }

                        // Guild Roles
                        foreach (var role in guild.Roles)
                        {
                            _cacheService.Runtime.SetValue(Constants.Cache.Keys.GuildRole, role, guild.Id + role.Id);
                        }

                        // Guild Presences
                        foreach (var presence in guild.Presences)
                        {
                            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
                        }

                        AddOrUpdateGuild(bGuild.Model.Id, bGuild);
                    }

                    MainViewModel.BindableGuilds.Add(GetGuild("DM"));
                    foreach (var folder in m.EventData.Settings.GuildFolders)
                    {
                        BindableGuildFolder bindableFolder = new BindableGuildFolder(folder) { IsCollapsed = folder.GuildIds.Count() > 1 };
                        MainViewModel.BindableGuilds.Add(bindableFolder);
                        foreach (var guildId in folder.GuildIds)
                        {
                            BindableGuild guild = GetGuild(guildId);
                            guild.FolderId = folder.Id;
                            guild.IsCollapsed = bindableFolder.IsCollapsed;
                            MainViewModel.BindableGuilds.Add(guild);
                        }
                    }
                });
            });

            Messenger.Default.Register<GatewayChannelCreatedMessage>(this, m =>
            {
                string guildId = "DM";
                if (m.Channel is GuildChannel gChannel)
                {
                    guildId = gChannel.GuildId;
                }

                var bChannel = new BindableChannel(m.Channel);
                if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                {
                    BindableChannel parentChannel = _channelsService.GetChannel(bChannel.ParentId);
                    bChannel.ParentPostion = parentChannel != null ? parentChannel.Position : 0;
                }
                else if (bChannel.ParentId == null)
                {
                    bChannel.ParentPostion = -1;
                }

                BindableGuild guild = GetGuild(guildId);
                if (guild != null)
                {
                    for (int i = 0; i < guild.Channels.Count; i++)
                    {
                        if (guild.Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                        {
                            _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                            {
                                guild.Channels.Insert(i, bChannel);
                            });
                            break;
                        }
                    }
                }

                _channelsService.AddOrUpdateChannel(bChannel.Model.Id, bChannel);
            });

            Messenger.Default.Register<GatewayChannelDeletedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableChannel currentChannel = _channelsService.GetChannel(m.Channel.Id);
                    if (currentChannel != null)
                    {
                        BindableGuild guild = GetGuild(currentChannel.GuildId);
                        if (guild != null)
                        {
                            guild.Channels.Remove(currentChannel);
                        }

                        _channelsService.RemoveChannel(m.Channel.Id);
                    }
                });
            });
            Messenger.Default.Register<GatewayGuildChannelUpdatedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    var bChannel = _channelsService.GetChannel(m.Channel.Id ?? "DM");
                    bChannel.Model = m.Channel;

                    if (bChannel.Model.Type != 4 && bChannel.ParentId != null)
                    {
                        BindableChannel parentChannel = _channelsService.GetChannel(bChannel.ParentId);
                        bChannel.ParentPostion = parentChannel != null ? parentChannel.Position : 0;
                    }
                    else if (bChannel.ParentId == null)
                    {
                        bChannel.ParentPostion = -1;
                    }

                    BindableGuild guild = GetGuild(m.Channel.GuildId);
                    if (guild != null)
                    {
                        guild.Channels.Remove(bChannel);
                        for (int i = 0; i < guild.Channels.Count; i++)
                        {
                            if (guild.Channels[i].AbsolutePostion > bChannel.AbsolutePostion)
                            {
                                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                                {
                                    guild.Channels.Insert(i, bChannel);
                                });
                                break;
                            }
                        }
                    }
                });
            });
            Messenger.Default.Register<GuildNavigateMessage>(this, m =>
            {
                if (CurrentGuild != null)
                {
                    CurrentGuild.Selected = false;
                }

                CurrentGuild = m.Guild;
                CurrentGuild.Selected = true;
            });
            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                _guildUsers.TryGetValue(m.GuildMembersChunk.GuildId, out var guild);
                foreach (var member in m.GuildMembersChunk.Members)
                {
                    guild.TryAdd(member.User.Id, new BindableGuildMember(member, m.GuildMembersChunk.GuildId));
                }

                if (m.GuildMembersChunk.Presences != null)
                {
                    foreach (var presence in m.GuildMembersChunk.Presences)
                    {
                        _presenceService.UpdateUserPrecense(presence.User.Id, presence);
                    }
                }
            });
        }

        /// <inheritdoc/>
        public BindableGuild CurrentGuild { get; private set; }

        private MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        private IVoiceService VoiceService => _voiceService ?? (_voiceService = SimpleIoc.Default.GetInstance<IVoiceService>());

        /// <inheritdoc/>
        public BindableGuild GetGuild(string guildId)
        {
            if (guildId == null)
            {
                return null;
            }

            return MainViewModel.AllGuilds.TryGetValue(guildId, out BindableGuild guild) ? guild : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateGuild(string guildId, BindableGuild guild)
        {
            MainViewModel.AllGuilds.AddOrUpdate(guildId, guild);
        }

        /// <inheritdoc/>
        public void RemoveGuild(string guildId)
        {
            MainViewModel.AllGuilds.Remove(guildId);
        }

        /// <inheritdoc/>
        public GuildSetting GetGuildSetting(string guildId)
        {
            if (guildId == null)
            {
                return null;
            }

            return MainViewModel.GuildSettings.TryGetValue(guildId, out GuildSetting guildSetting) ? guildSetting : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateGuildSettings(string guildId, GuildSetting guildSetting)
        {
            MainViewModel.GuildSettings.AddOrUpdate(guildId, guildSetting);
        }

        /// <inheritdoc/>
        public BindableGuildMember GetGuildMember(string memberId, string guildId)
        {
            if (_guildUsers.TryGetValue(guildId, out var guild) && guild.TryGetValue(memberId, out BindableGuildMember member))
            {
                return member;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public BindableGuildMember GetGuildMember(string username, string discriminator, string guildId)
        {
            if (_guildUsers.TryGetValue(guildId, out ConcurrentDictionary<string, BindableGuildMember> value))
            {
                return value.Values.FirstOrDefault(x => x.Model.User.Username == username && x.Model.User.Discriminator == discriminator);
            }

            return null;
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, BindableGuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId)
        {
            Dictionary<string, BindableGuildMember> guildMembers = new Dictionary<string, BindableGuildMember>();
            List<string> guildMembersToBeRequested = new List<string>();
            if (_guildUsers.TryGetValue(guildId, out var guild))
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

        /// <inheritdoc/>
        public IEnumerable<BindableGuildMember> QueryGuildMembers(string query, string guildId, int take = 10)
        {
            return _guildUsers[guildId].Values.Where(x => x.DisplayName.ToLower().StartsWith(query.ToLower()) || x.Model.User.Username.ToLower().StartsWith(query.ToLower())).Take(take);
        }
    }
}
