// Quarrel © 2022

using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the behaviors page in user settings.
    /// </summary>
    public class BehaviorPageViewModel : UserSettingsSubPageViewModel
    {
        private const string BehaviorResource = "UserSettings/Behavior";

        internal BehaviorPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[BehaviorResource];

        /// <inheritdoc/>
        public override void ResetValues()
        {
        }
    }
}
