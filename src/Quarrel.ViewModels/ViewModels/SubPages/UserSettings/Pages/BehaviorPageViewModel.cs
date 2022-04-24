// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class BehaviorPageViewModel : UserSettingsSubPageViewModel
    {
        private const string BehaviorResource = "UserSettings/Behavior";

        public BehaviorPageViewModel(ILocalizationService localizationService) :
            base(localizationService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[BehaviorResource];
    }
}
