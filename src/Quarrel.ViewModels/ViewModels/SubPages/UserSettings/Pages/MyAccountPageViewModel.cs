// Quarrel © 2022

using Quarrel.Bindables.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.UserSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    public class MyAccountPageViewModel : UserSettingsSubPageViewModel
    {
        private const string MyAccountResource = "UserSettings/MyAccount";
        private readonly IDiscordService _discordService;

        private string? _email;
        private string _username;
        private int _discriminator;
        private string? _aboutMe;

        internal MyAccountPageViewModel(ILocalizationService localizationService, IStorageService storageService, IDiscordService discordService) :
            base(localizationService, storageService)
        {
            _discordService = discordService;

            var user = _discordService.GetMe();
            if (user is not null)
            {
                SetBaseValues(user);
            }
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[MyAccountResource];

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public int Discriminator
        {
            get => _discriminator;
            set => SetProperty(ref _discriminator, value);
        }

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
