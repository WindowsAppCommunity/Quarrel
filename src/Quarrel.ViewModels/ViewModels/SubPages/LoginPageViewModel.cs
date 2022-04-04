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
        public enum LoginPageState
        {
            QuarrelPrompt,
            DiscordPrompt,
            TokenPrompt,
        }

        private readonly IAnalyticsService _analyticsService;
        private readonly IDiscordService _discordService;

        [AlsoNotifyChangeFor(nameof(IsShowingQuarrelPrompt))]
        [AlsoNotifyChangeFor(nameof(IsShowingDiscordPrompt))]
        [AlsoNotifyChangeFor(nameof(IsShowingTokenPrompt))]
        [ObservableProperty]
        private LoginPageState _pageState;

        [ObservableProperty]
        private string? _tokenText;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IAnalyticsService analyticsService, IDiscordService discordService)
        {
            _analyticsService = analyticsService;
            _discordService = discordService;

            ShowQuarrelPromptCommand = new RelayCommand(() => ShowQuarrelPrompt());
            ShowDiscordPromptCommand = new RelayCommand(() => ShowDiscordPrompt());
            ShowTokenPromptCommand = new RelayCommand(() => ShowTokenPrompt());
            LoginWithTokenCommand = new RelayCommand(() => LoginWithToken(TokenText));
            PageState = LoginPageState.QuarrelPrompt;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the generic login prompt is showing.
        /// </summary>
        public bool IsShowingQuarrelPrompt => _pageState == LoginPageState.QuarrelPrompt;

        /// <summary>
        /// Gets or sets a value indicating whether or not the Discord webview prompt is showing.
        /// </summary>
        public bool IsShowingDiscordPrompt => _pageState == LoginPageState.DiscordPrompt;

        /// <summary>
        /// Gets or sets a value indicating whether or not the token login prompt is showing.
        /// </summary>
        public bool IsShowingTokenPrompt => _pageState == LoginPageState.TokenPrompt;

        /// <summary>
        /// A command that shows the generic prompt.
        /// </summary>
        public IRelayCommand ShowQuarrelPromptCommand { get; }

        /// <summary>
        /// A command that shows the Discord webview prompt.
        /// </summary>
        public IRelayCommand ShowDiscordPromptCommand { get; }

        /// <summary>
        /// A command that shows the token login prompt.
        /// </summary>
        public IRelayCommand ShowTokenPromptCommand { get; }

        /// <summary>
        /// A command that logins in with the bound token.
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

        private void ShowQuarrelPrompt() => PageState = LoginPageState.QuarrelPrompt;

        private void ShowDiscordPrompt() => PageState = LoginPageState.DiscordPrompt;

        private void ShowTokenPrompt() => PageState = LoginPageState.TokenPrompt;
    }
}
