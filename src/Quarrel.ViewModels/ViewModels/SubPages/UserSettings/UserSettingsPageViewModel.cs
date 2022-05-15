// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings;
using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings
{
    /// <summary>
    /// A view model for the user settings page.
    /// </summary>
    public class UserSettingsPageViewModel : ObservableObject
    {
        private const string AccountSettingsResource = "UserSettings/AccountSettings";
        private const string AppSettingsResource = "UserSettings/AppSettings";

        private readonly ILocalizationService _localizationService;
        private UserSettingsSubPageViewModel? _selectedSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingsPageViewModel"/>.
        /// </summary>
        public UserSettingsPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService)
        {
            _localizationService = localizationService;

            Pages = new ObservableCollection<IUserSettingsMenuItem>();

            // Account settings
            Pages.Add(new UserSettingsHeader(localizationService, AccountSettingsResource));
            Pages.Add(new MyAccountPageViewModel(_localizationService, storageService, discordService));
            Pages.Add(new PrivacyPageViewModel(_localizationService, storageService, discordService));
            Pages.Add(new ConnectionsPageViewModel(_localizationService, storageService));

            // App Settings
            Pages.Add(new UserSettingsHeader(localizationService, AppSettingsResource));
            Pages.Add(new DisplayPageViewModel(_localizationService, storageService));
            Pages.Add(new BehaviorPageViewModel(_localizationService, storageService));
            Pages.Add(new NotificationsPageViewModel(_localizationService, storageService));
            Pages.Add(new VoicePageViewModel(_localizationService, storageService));
        }

        /// <summary>
        /// Gets the view model of the selected sub page.
        /// </summary>
        public UserSettingsSubPageViewModel? SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        /// <summary>
        /// Gets the view models of all subpage options.
        /// </summary>
        public ObservableCollection<IUserSettingsMenuItem> Pages { get; }
    }
}
