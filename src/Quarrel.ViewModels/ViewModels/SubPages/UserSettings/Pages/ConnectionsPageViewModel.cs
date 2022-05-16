// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// A view model for the connections page in settings.
    /// </summary>
    public class ConnectionsPageViewModel : UserSettingsSubPageViewModel
    {
        private const string ConnectionsResource = "UserSettings/Connections";

        internal ConnectionsPageViewModel(ILocalizationService localizationService, IStorageService storageService) :
            base(localizationService, storageService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[ConnectionsResource];
    }
}
