// Quarrel © 2022

using Discord.API.Models.Enums.Settings;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class PrivacyPageViewModel : UserSettingsSubPageViewModel
    {
        private const string PrivacyResource = "UserSettings/Privacy";
        private readonly IDiscordService _discordService;
        private ExplicitContentFilterLevel _contentFilterLevel;

        internal PrivacyPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService) :
            base(localizationService, storageService)
        {
            _discordService = discordService;
        }

        public override string Glyph => "";

        public override string Title => _localizationService[PrivacyResource];

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

        public bool FilterAll
        {
            get => _contentFilterLevel == ExplicitContentFilterLevel.All;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.All;
            }
        }

        public bool FilterPublic
        {
            get => _contentFilterLevel == ExplicitContentFilterLevel.Public;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.Public;
            }
        }

        public bool FilterNone
        {
            get => _contentFilterLevel == ExplicitContentFilterLevel.None;
            set
            {
                if (!value) return;
                ContentFilterLevel = ExplicitContentFilterLevel.None;
            }
        }
    }
}
