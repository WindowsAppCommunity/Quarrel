// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract
{
    public abstract class UserSettingsSubPageViewModel : ObservableObject
    {
        protected readonly ILocalizationService _localizationService;
        protected readonly IStorageService _storageService;

        public UserSettingsSubPageViewModel(ILocalizationService localizationService, IStorageService storageService)
        {
            _localizationService = localizationService;
            _storageService = storageService;
        }

        public abstract string Glyph { get; }

        public abstract string Title { get; }

        public virtual bool IsActive => false;
    }
}
