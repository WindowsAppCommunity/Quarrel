// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.DispatcherHelper;

namespace Quarrel.ViewModels.Services.Discord.CurrentUser
{
    /// <summary>
    /// Manages the all information directly pertaining to the current user.
    /// </summary>
    public class CurrentUsersService : ICurrentUserService
    {
        private readonly ICacheService _cacheService;
        private readonly IGuildsService _guildsService;
        private readonly IPresenceService _presenceService;
        private readonly IDispatcherHelper _dispatcherHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUsersService"/> class.
        /// </summary>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="guildsService">The app's guild service.</param>
        /// <param name="presenceService">The app's presence service.</param>
        /// <param name="dispatcherHelper">The app's dispatcher helper.</param>
        public CurrentUsersService(ICacheService cacheService, IGuildsService guildsService, IPresenceService presenceService, IDispatcherHelper dispatcherHelper)
        {
            _cacheService = cacheService;
            _guildsService = guildsService;
            _presenceService = presenceService;
            _dispatcherHelper = dispatcherHelper;

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    CurrentUser.Model = m.EventData.User;
                    CurrentUser.Presence = new DiscordAPI.Models.Presence()
                    {
                        User = null,
                        Game = null,
                        GuildId = null,
                        Roles = null,
                        Status = m.EventData.Settings.Status,
                    };

                    CurrentUserSettings = m.EventData.Settings;
                    _presenceService.UpdateUserPrecense(CurrentUser.Model.Id, CurrentUser.Presence);
                });
            });
            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, m =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    CurrentUserSettings = m.Settings;

                    if (!string.IsNullOrEmpty(m.Settings.Status))
                    {
                        var newPresence = new DiscordAPI.Models.Presence()
                        {
                            User = CurrentUser.Presence.User,
                            Game = CurrentUser.Presence.Game,
                            GuildId = CurrentUser.Presence.GuildId,
                            Roles = CurrentUser.Presence.Roles,
                            Status = m.Settings.Status,
                        };
                        CurrentUser.Presence = newPresence;
                        _presenceService.UpdateUserPrecense(CurrentUser.Model.Id, CurrentUser.Presence);
                    }
                });
            });
        }

        /// <inheritdoc/>
        public BindableUser CurrentUser { get; } = new BindableUser(new User());

        /// <inheritdoc/>
        public UserSettings CurrentUserSettings { get; private set; } = new UserSettings();
    }
}
