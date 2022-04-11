// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.Services.Analytics;
using Quarrel.Services.Discord;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// A ViewModel for the LoginPage.
    /// </summary>
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IDiscordService _discordService;

        [ObservableProperty]
        private string? _tokenText;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IAnalyticsService analyticsService, IDiscordService discordService)
        {
            _analyticsService = analyticsService;
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
        public void LoginWithToken(string? token)
        {
            if (token is null)
            {
                return;
            }

            _analyticsService.Log(Constants.Analytics.Events.LoggedInWithToken);
            _discordService.LoginAsync(token);
        }
    }
}
