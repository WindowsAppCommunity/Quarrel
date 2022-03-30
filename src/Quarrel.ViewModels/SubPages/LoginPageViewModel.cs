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

        private bool _isShowingGenericPrompt;
        private bool _isShowingDiscordPrompt;
        private bool _isShowingTokenPrompt;

        private string? _tokenText;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel"/> class.
        /// </summary>
        public LoginPageViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;

            ShowGenericPromptCommand = new RelayCommand(() => ShowGenericPrompt());
            ShowDiscordPromptCommand = new RelayCommand(() => ShowDiscordPrompt());
            ShowTokenPromptCommand = new RelayCommand(() => ShowTokenPrompt());
            LoginWithTokenCommand = new RelayCommand(() => LoginWithToken(TokenText));
            _isShowingGenericPrompt = true;
            _isShowingDiscordPrompt = false;
            _isShowingTokenPrompt = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the generic login prompt is showing.
        /// </summary>
        public bool IsShowingGenericPrompt
        {
            get => _isShowingGenericPrompt;
            private set
            {
                if (value)
                {
                    SetProperty(ref _isShowingGenericPrompt, value, nameof(IsShowingGenericPrompt));

                    SetProperty(ref _isShowingDiscordPrompt, false, nameof(IsShowingDiscordPrompt));
                    SetProperty(ref _isShowingTokenPrompt, false, nameof(IsShowingTokenPrompt));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Discord webview prompt is showing.
        /// </summary>
        public bool IsShowingDiscordPrompt
        {
            get => _isShowingDiscordPrompt;
            private set
            {
                if (value)
                {
                    SetProperty(ref _isShowingDiscordPrompt, value, nameof(IsShowingDiscordPrompt));

                    SetProperty(ref _isShowingGenericPrompt, false, nameof(IsShowingGenericPrompt));
                    SetProperty(ref _isShowingTokenPrompt, false, nameof(IsShowingTokenPrompt));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the token login prompt is showing.
        /// </summary>
        public bool IsShowingTokenPrompt
        {
            get => _isShowingTokenPrompt;
            private set
            {
                if (value)
                {
                    SetProperty(ref _isShowingTokenPrompt, value, nameof(IsShowingTokenPrompt));

                    SetProperty(ref _isShowingGenericPrompt, false, nameof(IsShowingGenericPrompt));
                    SetProperty(ref _isShowingDiscordPrompt, false, nameof(IsShowingDiscordPrompt));
                }
            }
        }

        /// <summary>
        /// Gets or sets the token text bound in the UI.
        /// </summary>
        public string? TokenText
        {
            get => _tokenText;
            set => SetProperty(ref _tokenText, value);
        }

        /// <summary>
        /// A command that shows the generic prompt.
        /// </summary>
        public IRelayCommand ShowGenericPromptCommand { get; }

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
            _analyticsService.Log(Constants.Analytics.Events.LoggedInWithToken);

            // TODO: Login to Discord service and cache token.
        }

        private void ShowGenericPrompt()
        {
            IsShowingGenericPrompt = true;
        }

        private void ShowDiscordPrompt()
        {
            IsShowingDiscordPrompt = true;
        }

        private void ShowTokenPrompt()
        {
            IsShowingTokenPrompt = true;
        }
    }
}
