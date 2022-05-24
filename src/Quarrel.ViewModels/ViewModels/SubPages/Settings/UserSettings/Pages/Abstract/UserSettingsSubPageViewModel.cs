// Quarrel © 2022

using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract
{
    /// <summary>
    /// A base class for user settings sub-page view models.
    /// </summary>
    public abstract class UserSettingsSubPageViewModel : SettingsSubPageViewModel
    {
        /// <summary>
        /// The discord service.
        /// </summary>
        protected readonly IDiscordService _discordService;

        /// <summary>
        /// The storage service.
        /// </summary>
        protected readonly IStorageService _storageService;

        internal UserSettingsSubPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService)
        {
            _discordService = discordService;
            _storageService = storageService;
        }
    }
}
