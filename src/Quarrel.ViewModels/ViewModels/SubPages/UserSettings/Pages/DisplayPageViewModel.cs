// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

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
    }
}
