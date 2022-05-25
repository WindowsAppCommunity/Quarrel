// Quarrel © 2022

using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings
{
    /// <summary>
    /// A view model for the user settings page.
    /// </summary>
    public class UserSettingsPageViewModel : SettingsPageViewModel
    {
        private const string AccountSettingsResource = "UserSettings/AccountSettings";
        private const string AppSettingsResource = "UserSettings/AppSettings";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsPageViewModel"/>.
        /// </summary>
        public UserSettingsPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService) :
            base(new ISettingsMenuItem[]
            {
                // Account settings
                new SettingsCategoryHeader(localizationService[AccountSettingsResource]),
                new MyAccountPageViewModel(localizationService, discordService, storageService),
                new PrivacyPageViewModel(localizationService, discordService, storageService),
                new ConnectionsPageViewModel(localizationService, discordService, storageService),

                // App Settings
                new SettingsCategoryHeader(localizationService[AppSettingsResource]),
                new DisplayPageViewModel(localizationService, discordService, storageService),
                new BehaviorPageViewModel(localizationService, discordService, storageService),
                new NotificationsPageViewModel(localizationService, discordService, storageService),
                new VoicePageViewModel(localizationService, discordService, storageService)
            })
        {
        }
    }
}
