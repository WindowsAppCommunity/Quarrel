// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages
{
    /// <summary>
    /// A view model for the account page in user settings.
    /// </summary>
    public class MyAccountPageViewModel : UserSettingsSubPageViewModel
    {
        private const string MyAccountResource = "UserSettings/MyAccount";

        private bool _isLoggedIn;
        private DraftValue<string?>? _email;
        private DraftValue<string?>? _username;
        private DraftValue<int>? _discriminator;
        private DraftValue<string?>? _aboutMe;

        internal MyAccountPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
            _isLoggedIn = false;

            var user = _discordService.GetMe();

            if (user is not null)
            {
                _isLoggedIn = true;

                Email = new(user.SelfUser.Email);
                Username = new(user.SelfUser.Username);
                Discriminator = new(user.SelfUser.Discriminator);
                AboutMe = new(user.SelfUser.Bio);

                RegisterDraftValues(Email, Username, Discriminator, AboutMe);
            }
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[MyAccountResource];

        /// <inheritdoc/>
        public override bool IsActive => _isLoggedIn;

        /// <summary>
        /// Gets or sets the current user's email.
        /// </summary>
        public DraftValue<string?>? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        /// <summary>
        /// Gets or sets the current user's username.
        /// </summary>
        public DraftValue<string?>? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Gets or sets the current user's discriminator.
        /// </summary>
        public DraftValue<int>? Discriminator
        {
            get => _discriminator;
            set => SetProperty(ref _discriminator, value);
        }

        /// <summary>
        /// Gets or sets the current user's about me description.
        /// </summary>
        public DraftValue<string?>? AboutMe
        {
            get => _aboutMe;
            set => SetProperty(ref _aboutMe, value);
        }

        /// <inheritdoc/>
        protected override void ApplyChanges()
        {
        }
    }
}
