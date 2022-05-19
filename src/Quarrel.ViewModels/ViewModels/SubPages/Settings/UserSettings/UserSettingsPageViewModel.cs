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
            Pages = new ObservableCollection<ISettingsMenuItem>();

            // Account settings
            Pages.Add(new SettingsCategoryHeader(localizationService, AccountSettingsResource));
            Pages.Add(new MyAccountPageViewModel(localizationService, storageService, discordService));
            Pages.Add(new PrivacyPageViewModel(localizationService, storageService, discordService));
            Pages.Add(new ConnectionsPageViewModel(localizationService, storageService));

            // App Settings
            Pages.Add(new SettingsCategoryHeader(localizationService, AppSettingsResource));
            Pages.Add(new DisplayPageViewModel(localizationService, storageService));
            Pages.Add(new BehaviorPageViewModel(localizationService, storageService));
            Pages.Add(new NotificationsPageViewModel(localizationService, storageService));
            Pages.Add(new VoicePageViewModel(localizationService, storageService));
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
