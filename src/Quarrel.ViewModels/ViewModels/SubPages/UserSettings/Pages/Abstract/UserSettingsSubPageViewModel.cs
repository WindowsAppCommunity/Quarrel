// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract
{
    public abstract class UserSettingsSubPageViewModel : ObservableObject
    {
        protected readonly ILocalizationService _localizationService;

        public UserSettingsSubPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public abstract string Glyph { get; }

        public abstract string Title { get; }
    }
}
