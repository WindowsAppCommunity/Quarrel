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

        [ObservableProperty]
        private LoginPageState _pageState;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IAnalyticsService analyticsService, IDiscordService discordService)
        {
            _analyticsService = analyticsService;
            _discordService = discordService;

            PageState = LoginPageState.Quarrel;
            LoginWithTokenCommand = new RelayCommand(() => LoginWithToken(TokenText));
        }

        public IRelayCommand LoginWithTokenCommand { get; }

        [ICommand]
        public void GoToDiscordLogin()
            => PageState = LoginPageState.Discord;

        [ICommand]
        public void GoToTokenLogin()
            => PageState = LoginPageState.Token;

        [ICommand]
        public void GoBack()
            => PageState = LoginPageState.Quarrel;

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
