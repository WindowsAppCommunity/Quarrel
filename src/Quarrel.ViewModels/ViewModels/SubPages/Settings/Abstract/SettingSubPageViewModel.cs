// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// A base class for settings sub-page view models.
    /// </summary>
    public abstract class SettingsSubPageViewModel : ObservableObject, ISettingsMenuItem
    {
        /// <summary>
        /// The localization service.
        /// </summary>
        protected readonly ILocalizationService _localizationService;

        /// <summary>
        /// The storage service.
        /// </summary>
        protected readonly IStorageService _storageService;

        internal SettingsSubPageViewModel(ILocalizationService localizationService, IStorageService storageService)
        {
            _localizationService = localizationService;
            _storageService = storageService;
        }

        /// <summary>
        /// Gets the string used as a glyph for the sub page.
        /// </summary>
        public abstract string Glyph { get; }

        /// <summary>
        /// Gets the title of the sub page.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets whether or not the page is currently active.
        /// </summary>
        public virtual bool IsActive => false;
    }
}
