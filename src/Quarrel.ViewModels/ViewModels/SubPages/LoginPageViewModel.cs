// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// A ViewModel for the LoginPage.
    /// </summary>
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IDiscordService _discordService;
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private string? _tokenText;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IDiscordService discordService, ILocalizationService localizationService)
        {
            _discordService = discordService;
            _localizationService = localizationService;

            LoginWithTokenCommand = new RelayCommand(() => LoginWithToken(TokenText));
        }

        /// <summary>
        /// Gets a relay command that logs in with the current text in the <see cref="TokenText"/>.
        /// </summary>
        public IRelayCommand LoginWithTokenCommand { get; }

        /// <summary>
        /// Gets a value indicating whether or not the app's current language is the neutral language.
        /// </summary>
        public bool IsNeutralLanguage => _localizationService.IsNeutralLanguage;

        /// <summary>
        /// Logs the user in with a token.
        /// </summary>
        public async void LoginWithToken(string? token)
        {
            if (token is null)
            {
                return;
            }

            await _discordService.LoginAsync(token, LoginType.Fresh);
        }
    }
}
