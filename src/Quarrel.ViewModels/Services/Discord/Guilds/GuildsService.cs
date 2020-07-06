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
        private readonly IChannelsService _channelsService;
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IPresenceService _presenceService;
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
            IChannelsService channelsService,
            IDispatcherHelper dispatcherHelper,
            IPresenceService presenceService)
        {
            _channelsService = channelsService;
            _dispatcherHelper = dispatcherHelper;
            _presenceService = presenceService;
        }
        
        /// <inheritdoc/>
        public BindableGuild CurrentGuild
        {
            get => MainViewModel.CurrentGuild;
            set => MainViewModel.CurrentGuild = value;
        }

        public IDictionary<string, ConcurrentDictionary<string, BindableGuildMember>> GuildUsers { get; } =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, BindableGuildMember>>();

        private MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

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
            if (GuildUsers.TryGetValue(guildId, out var guild) && guild.TryGetValue(memberId, out BindableGuildMember member))
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
            if (GuildUsers.TryGetValue(guildId, out ConcurrentDictionary<string, BindableGuildMember> value))
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
            if (GuildUsers.TryGetValue(guildId, out var guild))
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
            return GuildUsers[guildId].Values.Where(x => x.DisplayName.ToLower().StartsWith(query.ToLower()) || x.Model.User.Username.ToLower().StartsWith(query.ToLower())).Take(take);
        }
    }
}
