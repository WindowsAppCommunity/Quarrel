// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the display page in settings.
    /// </summary>
    public class DisplayPageViewModel : SettingsSubPageViewModel
    {
        private const string ConnectionsResource = "UserSettings/Display";

        internal DisplayPageViewModel(ILocalizationService localizationService, IStorageService storageService) :
            base(localizationService, storageService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[ConnectionsResource];

        /// <inheritdoc/>
        public override bool IsActive => true;

        /// <summary>
        /// Gets the app selected language.
        /// </summary>
        public string SelectedLanguage
        {
            get => _localizationService.LanguageOverride;
            set => _localizationService.LanguageOverride = value;
        }

        /// <summary>
        /// Gets the list of languages available for the app.
        /// </summary>
        public IReadOnlyList<string> LanguageOptions => _localizationService.AvailableLanguages;
    }
}
