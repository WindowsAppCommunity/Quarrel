// Quarrel © 2022

using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class VoicePageViewModel : UserSettingsSubPageViewModel
    {
        private const string VoiceResource = "UserSettings/Voice";

        public VoicePageViewModel(ILocalizationService localizationService) :
            base(localizationService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[VoiceResource];
    }
}
