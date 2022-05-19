// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings
{
    /// <summary>
    /// A view model for the user settings page.
    /// </summary>
    public class UserSettingsPageViewModel : ObservableObject
    {
        private const string AccountSettingsResource = "UserSettings/AccountSettings";
        private const string AppSettingsResource = "UserSettings/AppSettings";

        private SettingsSubPageViewModel? _selectedSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsPageViewModel"/>.
        /// </summary>
        public UserSettingsPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService)
        {
            Pages = new ObservableCollection<ISettingsMenuItem>
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
            };
        }

        /// <summary>
        /// Gets the view model of the selected sub page.
        /// </summary>
        public SettingsSubPageViewModel? SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        /// <summary>
        /// Gets the view models of all subpage options.
        /// </summary>
        public ObservableCollection<ISettingsMenuItem> Pages { get; }
    }
}
