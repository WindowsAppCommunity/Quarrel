// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;
using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class DisplayPageViewModel : UserSettingsSubPageViewModel
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

        public string SelectedLanguage
        {
            get => _localizationService.LanguageOverride;
            set => _localizationService.LanguageOverride = value;
        }

        public IReadOnlyList<string> LanguageOptions => _localizationService.AvailableLanguages;
    }
}
