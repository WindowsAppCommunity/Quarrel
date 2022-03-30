// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quarrel.ViewModels.Services.Analytics;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// A ViewModel for the LoginPage.
    /// </summary>
    public class LoginPageViewModel : ObservableObject
    {
        private readonly IAnalyticsService _analyticsService;

        private bool _isShowingDiscordPrompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;

            ShowDiscordPromptCommand = new RelayCommand(() => ShowPrompt());
            _isShowingDiscordPrompt = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Discord webview prompt is showing.
        /// </summary>
        public bool IsShowingDiscordPrompt
        {
            get => _isShowingDiscordPrompt;
            set => SetProperty(ref _isShowingDiscordPrompt, value);
        }

        /// <summary>
        /// A command that shows the Discord webview prompt.
        /// </summary>
        public IRelayCommand ShowDiscordPromptCommand { get; }

        /// <summary>
        /// Logs the user in with a token.
        /// </summary>
        public void LoginWithToken(string token)
        {
            _analyticsService.Log(Constants.Analytics.Events.LoggedInWithToken);

            // TODO: Login to Discord service and cache token.
        }

        private void ShowPrompt()
        {
            IsShowingDiscordPrompt = true;
        }
    }
}
