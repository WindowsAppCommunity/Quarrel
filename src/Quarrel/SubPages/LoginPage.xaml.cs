using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Quarrel.SubPages.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

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
            Logger.LogInformation("ScriptNotify" +
                $"\n\tsender: {sender}" +
                $"\n\te.CallingUri: {e.CallingUri}" +
                $"\n\te.Value: {e.Value}");

            if(e.Value.StartsWith("token:"))
            {
                var token = e.Value.Substring(e.Value.IndexOf(":") + 1).Trim('"');

                Logger.LogInformation($"GetTokenAndLogin - token: [{token}]");

                if (!string.IsNullOrEmpty(token))
                {
                    Logger.LogInformation($"GetTokenAndLogin - Logging in.");
                    discordService.Login(token);
                    subFrameNavigationService.GoBack();
                }

                return;
            }

            if (e.CallingUri.AbsolutePath == "/app")
                GetTokenAndLogin(true);
        }

        public bool Hideable { get; } = false;

        private void GetTokenAndLogin(bool notify)
        {
            Task<string> task = null;

            Logger.LogInformation($"GetTokenAndLogin - Injecting JavaScript to extract token.");

            // Respond to the script notification.
            try
            {
                var js = $"\nvar notify = {notify};\n{GetTokenFunction}";

                task = CaptchaView.InvokeScriptAsync("eval", new[] { js })
                    .AsTask<string>();

                var awaiter = task.GetAwaiter();

                awaiter.OnCompleted(() =>
                {
                    var token = awaiter.GetResult();
                    ProcessLogin(token);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(), ex, $"GetTokenAndLogin - Error" +
                    $"\n\ttask.Result: {task?.Result}" +
                    $"\n\ttask.Status: {task?.Status}");
            }
        }

        private string GetTokenFunction => @"    
    var result = '';

    try {
        var iframe = document.createElement('iframe');
        document.head.append(iframe);

        result =iframe.contentWindow.localStorage.getItem('token');

        if(notify) window.external.notify('token:' + result);
    
        result;
    } catch (ex) {
        window.external.notify(ex); 
    }
";

        private async void CaptchaView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Logger.LogInformation($"CaptchaView_OnNavigationCompleted" +
                $"\n\targs.Uri: {args.Uri}" +
                $"\n\targs.IsSuccess: {args.IsSuccess}" +
                $"\n\targs.WebErrorStatus: { args.WebErrorStatus}");

            if (args.Uri.AbsolutePath != "/app")
            {
                var script = new[] { $@"
try {{
    var pushState = history.pushState;  
    if(!pushState) {{ window.external.notify('pushState is undefined'); }}

    history.pushState = function () {{
        pushState.apply(history, arguments);	
        window.external.notify('pushState called.'); 
    }}

    'Injected to: {args.Uri}'
}} catch(e) {{ 
    window.external.notify(e); 

    e.toString();
}}
                " };
                            
                Logger.LogInformation($"CaptchaView_OnNavigationCompleted result: {await sender.InvokeScriptAsync("eval", script)}");

                return;
            }
            else
            {
                GetTokenAndLogin(false);
            }
        }

        private void ProcessLogin(string token)
        {
            Logger.LogInformation($"GetTokenAndLogin - token: [{token}]");

            if (!string.IsNullOrEmpty(token))
            {
                Logger.LogInformation($"GetTokenAndLogin - Logging in.");
                discordService.Login(token);
                subFrameNavigationService.GoBack();
            }

            return;
        }
    }
}
