// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Bindables.Users;
using Quarrel.Client;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Clipboard;
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
        private readonly IClipboardService _clipboardService;
        private readonly QuarrelClient _quarrelClient;

        private readonly bool _isLoggedIn;
        private readonly ulong? _userId;
        private DraftValue<string?>? _email;
        private DraftValue<string?>? _username;
        private DraftValue<int>? _discriminator;
        private DraftValue<string?>? _aboutMe;

        internal MyAccountPageViewModel(
            ILocalizationService localizationService,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IStorageService storageService,
            IClipboardService clipboardService) :
            base(localizationService, discordService, storageService)
        {
            _quarrelClient = quarrelClient;
            _clipboardService = clipboardService;

            _isLoggedIn = false;
            _userId = null;
            
            SelfUser? user = _quarrelClient.Self.CurrentUser;


            if (user is not null)
            {
                _isLoggedIn = true;
                _userId = user.Id;

                Email = new DraftValue<string?>(user.Email);
                Username = new DraftValue<string?>(user.Username);
                Discriminator = new DraftValue<int>(user.Discriminator);
                AboutMe = new DraftValue<string?>(user.Bio);

                RegisterDraftValues(AboutMe);
            }

            CopyIdCommand = new RelayCommand(CopyId);
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

        /// <summary>
        /// Gets a command that copies the user's id to the clipboard.
        /// </summary>
        public RelayCommand CopyIdCommand { get; }

        /// <inheritdoc/>
        protected override async void ApplyChanges()
        {
            var modify = new ModifySelfUser()
            {
                AboutMe = AboutMe?.EditedValue,
            };

            await _discordService.ModifyMe(modify);
        }

        private void CopyId()
        {
            if (_userId.HasValue)
            {
                _clipboardService.Copy($"{_userId.Value}");
            }
        }
    }
}
