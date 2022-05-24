// Quarrel © 2022

using Discord.API.Models.Enums.Settings;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the privacy page in user settings.
    /// </summary>
    public class PrivacyPageViewModel : UserSettingsSubPageViewModel
    {
        private const string PrivacyResource = "UserSettings/Privacy";
        private ExplicitContentFilterLevel _contentFilterLevel;

        internal PrivacyPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[PrivacyResource];

        /// <inheritdoc/>
        public override bool IsActive => true;

        private ExplicitContentFilterLevel ContentFilterLevel
        {
            get => _contentFilterLevel;
            set
            {
                if (SetProperty(ref _contentFilterLevel, value))
                {
                    OnPropertyChanged(nameof(FilterNone));
                    OnPropertyChanged(nameof(FilterPublic));
                    OnPropertyChanged(nameof(FilterAll));
                }
            }
        }

        /// <summary>
        /// Gets or sets if the content filter level is all.
        /// </summary>
        /// <remarks>
        /// Can only be set to <see langword="true"/>, clearing public and none.
        /// </remarks>
        public bool FilterAll
        {
            get => ContentFilterLevel == ExplicitContentFilterLevel.All;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.All;
            }
        }

        /// <summary>
        /// Gets or sets if the content filter level is public.
        /// </summary>
        /// <remarks>
        /// Can only be set to <see langword="true"/>, clearing all and none.
        /// </remarks>
        public bool FilterPublic
        {
            get => ContentFilterLevel == ExplicitContentFilterLevel.Public;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.Public;
            }
        }

        /// <summary>
        /// Gets or sets if the content filter level is none.
        /// </summary>
        /// <remarks>
        /// Can only be set to <see langword="true"/>, clearing all and public.
        /// </remarks>
        public bool FilterNone
        {
            get => ContentFilterLevel == ExplicitContentFilterLevel.None;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.None;
            }
        }
    }
}
