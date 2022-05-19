// Quarrel © 2022

using Quarrel.Bindables.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
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
        private string? _email;
        private string? _username;
        private int _discriminator;
        private string? _aboutMe;

        internal MyAccountPageViewModel(ILocalizationService localizationService, IDiscordService discordService, IStorageService storageService) :
            base(localizationService, discordService, storageService)
        {
            _isLoggedIn = false;

            var user = _discordService.GetMe();

            if (user is not null)
            {
                _isLoggedIn = true;
                SetBaseValues(user);
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
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        /// <summary>
        /// Gets or sets the current user's username.
        /// </summary>
        public string? Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Gets or sets the current user's discriminator.
        /// </summary>
        public int Discriminator
        {
            get => _discriminator;
            set => SetProperty(ref _discriminator, value);
        }

        /// <summary>
        /// Gets or sets the current user's about me description.
        /// </summary>
        public string? AboutMe
        {
            get => _aboutMe;
            set => SetProperty(ref _aboutMe, value);
        }

        private void SetBaseValues(BindableSelfUser user)
        {
            Email = user.SelfUser.Email;
            Username = user.SelfUser.Username;
            Discriminator = user.SelfUser.Discriminator;
            AboutMe = user.SelfUser.Bio;
        }
    }
}
