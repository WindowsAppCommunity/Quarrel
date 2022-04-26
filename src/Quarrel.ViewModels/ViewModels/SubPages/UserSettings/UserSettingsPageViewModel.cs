// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings
{
    public class UserSettingsPageViewModel : ObservableObject
    {
        private readonly ILocalizationService _localizationService;

        private UserSettingsSubPageViewModel _selectedSubPage;

        public UserSettingsPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService)
        {
            _localizationService = localizationService;

            Pages = new ObservableCollection<UserSettingsSubPageViewModel>();

            Pages.Add(new MyAccountPageViewModel(localizationService, storageService, discordService));
            Pages.Add(new PrivacyPageViewModel(_localizationService, storageService, discordService));
            Pages.Add(new ConnectionsPageViewModel(_localizationService, storageService));

            Pages.Add(new DisplayPageViewModel(_localizationService, storageService));
            Pages.Add(new BehaviorPageViewModel(_localizationService, storageService));
            Pages.Add(new NotificationsPageViewModel(_localizationService, storageService));
            Pages.Add(new VoicePageViewModel(_localizationService, storageService));
        }

        public UserSettingsSubPageViewModel SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        public ObservableCollection<UserSettingsSubPageViewModel> Pages { get; }
    }
}
