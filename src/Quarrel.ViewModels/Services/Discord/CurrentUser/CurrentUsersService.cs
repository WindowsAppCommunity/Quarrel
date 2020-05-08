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
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUsersService"/> class.
        /// </summary>
        public CurrentUsersService()
        {
            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
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
                    PresenceService.UpdateUserPrecense(CurrentUser.Model.Id, CurrentUser.Presence);
                });
            });
            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
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
                        PresenceService.UpdateUserPrecense(CurrentUser.Model.Id, CurrentUser.Presence);
                    }
                });
            });
        }

        /// <inheritdoc/>
        public BindableUser CurrentUser { get; } = new BindableUser(new User());

        /// <inheritdoc/>
        public UserSettings CurrentUserSettings { get; private set; } = new UserSettings();

        private ICacheService CacheService { get; } = SimpleIoc.Default.GetInstance<ICacheService>();

        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        private IPresenceService PresenceService { get; } = SimpleIoc.Default.GetInstance<IPresenceService>();

        private IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();
    }
}
