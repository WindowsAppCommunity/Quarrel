using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.CurrentUser
{
    public class CurrentUsersService : ICurrentUserService
    {
        public ICacheService CacheService;
        public IGuildsService GuildsService;
        private IPresenceService PresenceService;
        private IDispatcherHelper DispatcherHelper;

        public BindableUser CurrentUser { get; } = new BindableUser(new User());

        public UserSettings CurrentUserSettings { get; private set; } = new UserSettings();

        public CurrentUsersService(ICacheService cacheService, IDispatcherHelper dispatcherHelper, IGuildsService guildsService, IPresenceService presenceService)
        {
            CacheService = cacheService;
            DispatcherHelper = dispatcherHelper;
            GuildsService = guildsService;
            PresenceService = presenceService;

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
                        Status = m.EventData.Settings.Status
                    };

                    CurrentUserSettings = m.EventData.Settings;
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
                            Status = m.Settings.Status
                        };
                        CurrentUser.Presence = newPresence;
                    }
                });
            });
        }
    }
}
