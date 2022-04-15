// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Discord;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// A ViewModel for the LoginPage.
    /// </summary>
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IDiscordService _discordService;

        [ObservableProperty]
        private string? _tokenText;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IDiscordService discordService)
        {
            _discordService = discordService;

            LoginWithTokenCommand = new RelayCommand(() => LoginWithToken(TokenText));
        }

        /// <summary>
        /// Gets a relay command that logs in with the current text in the <see cref="TokenText"/>.
        /// </summary>
        public IRelayCommand LoginWithTokenCommand { get; }

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
