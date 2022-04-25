// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class PrivacyPageViewModel : UserSettingsSubPageViewModel
    {
        private const string PrivacyResource = "UserSettings/Privacy";

        internal PrivacyPageViewModel(ILocalizationService localizationService, IStorageService storageService) :
            base(localizationService, storageService)
        {
        }

        public override string Glyph => "";

        public override string Title => _localizationService[PrivacyResource];
    }
}
