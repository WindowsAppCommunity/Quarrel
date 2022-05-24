// Quarrel © 2022

using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the display page in user settings.
    /// </summary>
    public class DisplayPageViewModel : UserSettingsSubPageViewModel
    {
        private const string ConnectionsResource = "UserSettings/Display";

        internal DisplayPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
            SelectedLanguage = CultureInfo.GetCultureInfo(_localizationService.LanguageOverride);
            LanguageOptions = _localizationService.AvailableLanguages.Select(x => CultureInfo.GetCultureInfo(x));
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
        public CultureInfo SelectedLanguage
        {
            get => CultureInfo.GetCultureInfo(_localizationService.LanguageOverride);
            set => _localizationService.LanguageOverride = value.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Gets the list of languages available for the app.
        /// </summary>
        public IEnumerable<CultureInfo> LanguageOptions { get; }
    }
}
