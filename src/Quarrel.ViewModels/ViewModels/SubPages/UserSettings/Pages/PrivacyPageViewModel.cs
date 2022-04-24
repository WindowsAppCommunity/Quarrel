// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class PrivacyPageViewModel : UserSettingsSubPageViewModel
    {
        private const string PrivacyResource = "UserSettings/Privacy";

        public PrivacyPageViewModel(ILocalizationService localizationService) :
            base(localizationService)
        {
        }

        public override string Glyph => "";

        public override string Title => _localizationService[PrivacyResource];
    }
}
