// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to show the login prompt.
    /// </summary>
    public sealed partial class LoginPage : UserControl, IFullscreenSubPage
    {
        private IAnalyticsService _analyticsService = null;
        private IDiscordService _discordService = null;
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        public LoginPage()
        {
            this.InitializeComponent();
            AnalyticsService.Log(Constants.Analytics.Events.LoginOpened);
        }

        /// <inheritdoc/>
        public bool Hideable { get; } = false;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Collapsed;
            CaptchaView.Visibility = Visibility.Visible;
            CaptchaView.Navigate(new Uri("https://discord.com/app"));
        }

        private void LoginWithToken_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Visible;
            CaptchaView.Visibility = Visibility.Collapsed;

            // TODO: Login with token page
        }

        private async void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.com/"));
        }

        private void Token_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginButton_Click(null, null);
            }
        }

        private void MFAsms_Click(object sender, RoutedEventArgs e)
        {
            // TODO: MFA sms
        }

        private async void ScriptNotify(object sender, NotifyEventArgs e)
        {
            // Respond to the script notification.
            if (e.CallingUri.AbsolutePath == "/app")
            {
                string token = await GetTokenFromWebView();
                if (!string.IsNullOrEmpty(token))
                {
                    await DiscordService.Login(token, true);
                    SubFrameNavigationService.GoBack();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Raw string")]
        private async void CaptchaView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            _ = sender.InvokeScriptAsync(
                "eval",
                new[]
                {
                @"
                    var pushState = history.pushState;
                    history.pushState = function () {
                        pushState.apply(history, arguments);
	                    window.external.notify('');
                    };
                ",
                });
            if (args.Uri.AbsolutePath == "/app")
            {
                string token = await GetTokenFromWebView();
                if (!string.IsNullOrEmpty(token))
                {
                    await DiscordService.Login(token, true);
                    SubFrameNavigationService.GoBack();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Raw string")]
        private async Task<string> GetTokenFromWebView()
        {
            // Discord doesn't allow access to localStorage so create an iframe to bypass this.
            string token = await CaptchaView.InvokeScriptAsync(
                "eval",
                new[]
                {
                    @"
                    iframe = document.createElement('iframe');
                    document.body.appendChild(iframe);
                    iframe.contentWindow.localStorage.getItem('token');
                    //'<<token>>'",
                });

            AnalyticsService.Log(Constants.Analytics.Events.TokenIntercepted);

            return string.IsNullOrEmpty(token) ? string.Empty : token.Trim('"');
        }
    }
}
