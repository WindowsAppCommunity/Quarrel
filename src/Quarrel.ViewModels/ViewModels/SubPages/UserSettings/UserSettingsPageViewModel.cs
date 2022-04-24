// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings
{
    public class UserSettingsPageViewModel : ObservableObject
    {
        private readonly ILocalizationService _localizationService;

        private UserSettingsSubPageViewModel _selectedSubPage;

        public UserSettingsPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            Pages = new ObservableCollection<UserSettingsSubPageViewModel>();
            Pages.Add(new MyAccountPageViewModel(_localizationService));
            Pages.Add(new PrivacyPageViewModel(_localizationService));
            Pages.Add(new ConnectionsPageViewModel(_localizationService));
            Pages.Add(new DisplayPageViewModel(_localizationService));
            Pages.Add(new BehaviorPageViewModel(_localizationService));
            Pages.Add(new NotificationsPageViewModel(_localizationService));
            Pages.Add(new VoicePageViewModel(_localizationService));
        }

        public UserSettingsSubPageViewModel SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        public ObservableCollection<UserSettingsSubPageViewModel> Pages { get; }
    }
}
