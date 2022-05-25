// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Settings;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Client.Models.Settings;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the privacy page in user settings.
    /// </summary>
    public class PrivacyPageViewModel : UserSettingsSubPageViewModel
    {
        private const string PrivacyResource = "UserSettings/Privacy";

        private bool _isLoggedIn;
        private DraftValue<ExplicitContentFilterLevel>? _explicitContentFilterLevel;

        internal PrivacyPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
            _isLoggedIn = false;
            var settings = _discordService.GetSettings();

            if (settings is not null)
            {
                _isLoggedIn = true;

                ExplicitContentFilterLevel = new(settings.ContentFilterLevel);

                RegisterEvents();
            }

            SetExplicitContentFilterLevelCommand = new RelayCommand<ExplicitContentFilterLevel>(SetExplicitContentFilterLevel);
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[PrivacyResource];

        /// <inheritdoc/>
        public override bool IsActive => _isLoggedIn;

        /// <summary>
        /// Gets or sets the explicit content filter level.
        /// </summary>
        public DraftValue<ExplicitContentFilterLevel>? ExplicitContentFilterLevel
        {
            get => _explicitContentFilterLevel;
            set => SetProperty(ref _explicitContentFilterLevel, value);
        }

        /// <summary>
        /// Gets a command that sets the verification level.
        /// </summary>
        public RelayCommand<ExplicitContentFilterLevel> SetExplicitContentFilterLevelCommand { get; }

        private void SetExplicitContentFilterLevel(ExplicitContentFilterLevel explicitContentFilterLevel)
        {
            if (ExplicitContentFilterLevel is not null)
                ExplicitContentFilterLevel.Value = explicitContentFilterLevel;
        }

        /// <inheritdoc/>
        public override async void ApplyChanges()
        {
            ModifyUserSettings modify = new ModifyUserSettings()
            {
                ExplicitContentFilterLevel = ExplicitContentFilterLevel?.Value,
            };

            await _discordService.ModifySettings(modify);

            Guard.IsNotNull(ExplicitContentFilterLevel, nameof(ExplicitContentFilterLevel));
            ExplicitContentFilterLevel.CanonicalValue = ExplicitContentFilterLevel.Value;

            base.ApplyChanges();
        }

        /// <inheritdoc/>
        public override void RevertChanges()
        {
            ExplicitContentFilterLevel?.Reset();

            base.RevertChanges();
        }

        private void RegisterEvents()
        {
            Guard.IsNotNull(ExplicitContentFilterLevel);
            ExplicitContentFilterLevel.ValueChanged += ValueChanged;
        }
    }
}
