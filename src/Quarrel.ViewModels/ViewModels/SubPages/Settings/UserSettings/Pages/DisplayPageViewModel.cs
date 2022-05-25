// Quarrel © 2022

using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
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

        private DraftValue<CultureInfo> _selectedLanguage;

        internal DisplayPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
            _selectedLanguage = new(CultureInfo.GetCultureInfo(_localizationService.LanguageOverride));
            LanguageOptions = _localizationService.AvailableLanguages.Select(x => CultureInfo.GetCultureInfo(x));

            RegisterEvents();
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[ConnectionsResource];

        /// <inheritdoc/>
        public override bool IsActive => true;

        /// <summary>
        /// Gets or sets the app selected language.
        /// </summary>
        public DraftValue<CultureInfo> SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        /// <summary>
        /// Gets the list of languages available for the app.
        /// </summary>
        public IEnumerable<CultureInfo> LanguageOptions { get; }

        /// <inheritdoc/>
        public override void ApplyChanges()
        {
        }

        /// <inheritdoc/>
        public override void RevertChanges()
        {
            SelectedLanguage.Reset();
        }

        private void RegisterEvents()
        {
            SelectedLanguage.ValueChanged += ValueChanged;
        }
    }
}
