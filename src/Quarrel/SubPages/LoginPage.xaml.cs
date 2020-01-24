using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Rest;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Quarrel.SubPages
{
    public sealed partial class LoginPage : UserControl, IFullscreenSubPage
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISubFrameNavigationService subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();
        private ILogger Logger => App.ServiceProvider.GetService<ILogger<LoginPage>>();

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Collapsed;
            CaptchaView.Visibility = Visibility.Visible;
            CaptchaView.Navigate(new Uri("https://discordapp.com/login"));
        }

        private void LoginWithToken_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Visibility = Visibility.Visible;
            CaptchaView.Visibility = Visibility.Collapsed;
            // TODO: Login with token page
        }

        private async void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discordapp.com/"));
        }

        private void Token_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                LoginButton_Click(null, null);
        }

        private void MFAsms_Click(object sender, RoutedEventArgs e)
        {
            // TODO: MFA sms
        }

        private async void ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.CallingUri.AbsolutePath == "/app")
            {
                string token = await GetTokenFromWebView();
                if (!string.IsNullOrEmpty(token))
                {
                    await discordService.Login(token, true);
                    subFrameNavigationService.GoBack();
                }
            }
            // Respond to the script notification.
        }
        public bool Hideable { get; } = false;

        private async void CaptchaView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            _ = sender.InvokeScriptAsync("eval", new[]
            {
                @"
                    var pushState = history.pushState;
                    history.pushState = function () {
                        pushState.apply(history, arguments);
	                    window.external.notify('');
                    };
            "
            });
            if (args.Uri.AbsolutePath == "/app")
            {
                string token = await GetTokenFromWebView();
                if (!string.IsNullOrEmpty(token))
                {
                    await discordService.Login(token, true);
                    subFrameNavigationService.GoBack();
                }
            }
        }
        private async Task<string> GetTokenFromWebView()
        {

            //Discord doesn't allow access to localStorage so create an iframe to bypass this.
            string token = await CaptchaView.InvokeScriptAsync("eval", new[] { @"
                    iframe = document.createElement('iframe');
                    document.body.appendChild(iframe);
                    iframe.contentWindow.localStorage.getItem('token');
                //'<<token>>'
"
            });

            return string.IsNullOrEmpty(token) ? "" : token.Trim('"');
        }
    }

}
