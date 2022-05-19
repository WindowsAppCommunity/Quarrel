// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the behaviors page in settings.
    /// </summary>
    public class BehaviorPageViewModel : SettingsSubPageViewModel
    {
        private const string BehaviorResource = "UserSettings/Behavior";

        internal BehaviorPageViewModel(ILocalizationService localizationService, IStorageService storageService) :
            base(localizationService, storageService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[BehaviorResource];
    }
}
