using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;
using DiscordAPI.API.Login.Models;
using Refit;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogScreen : Page
    {
        public LogScreen()
        {
            this.InitializeComponent();
        }

        string cusername;
        string cpassword;

        string mfaTicket;

        private async void LogIn(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;
            ProgressRing.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;
            LoginText.Visibility = Visibility.Collapsed;

            if(NormalAuth.Visibility == Visibility.Visible)
            {
                //NORMAL AUTHENTICATION
                LoginResult result = new LoginResult();
                var username = Username.Text;
                var password = Password.Password;
                await Task.Run(async () =>
                {
                    result = await RESTCalls.Login(username, password);
                    if (result.exception != null)
                    {
                        LoginResult cache = result; 
                        result = JsonConvert.DeserializeObject<LoginResult>(((ApiException)result.exception).Content);
                        result.exception = cache.exception;
                    }
                });
                if(result.CaptchaKey != null)
                {
                    CaptchaView.Visibility = Visibility.Visible;
                    CaptchaView.Navigate(new Uri("https://discordapp.com/login"));
                    cusername = username;
                    cpassword = password;
                    MessageDialog md = new MessageDialog("Login in Failed, try again here", "Discord thinks you're a bot!");
                    await md.ShowAsync();
                }
                else if (result.Email != null || result.Password != null)
                {
                    if (result.Email != null)
                    {
                        EmailErrorText.Text = "";
                        for (int i = 0; i < result.Email.Count; i++)
                        {
                            EmailErrorText.Text += result.Email[i];
                            if (i != result.Email.Count - 1)
                            {
                                EmailErrorText.Text += Environment.NewLine;
                            }
                        }

                        EmailErrorText.Visibility = Visibility.Visible;
                    }
                    if (result.Password != null)
                    {
                        PasswordErrorText.Text = "";
                        for (int i = 0; i < result.Password.Count; i++)
                        {
                            PasswordErrorText.Text += result.Password[i];
                            if (i != result.Password.Count-1)
                            {
                                PasswordErrorText.Text += Environment.NewLine;
                            }
                        }

                        PasswordErrorText.Visibility = Visibility.Visible;
                    }
                    
                }
                else if (result.exception != null)
                {
                    string ermsg = "";
                    switch (result.exception.Message)
                    {
                        case "Response status code does not indicate success: 400 ().":
                        case "Response status code does not indicate success: 400().":
                            ermsg = "Response code (from Discord Servers) indicates failure (400), please check your email for an email from Discord to verify your IP";
                            break;
                        case "TBD":
                            ermsg = "A bug in the code is preventing log in, some imported code (refit) isn't working, trying again later may work";
                            break;
                        case "ILoginService doesn't look like a Refit interface. Make sure it has at least one method with a Refit HTTP method attribute and Refit is installed in the project.":
                            ermsg = "A bug in the code is preventing log in. Some imported code (Refit) isn't working as expected";
                            break;
                        default:
                            ermsg = "Unknown error, maybe try again later";
                            break;
                    }
                    MessageDialog msg = new MessageDialog(ermsg);
                    await msg.ShowAsync();
                }

                else if (result.MFA == true)
                {
                    mfaTicket = result.Ticket;
                    NormalAuth.Visibility = Visibility.Collapsed;
                    MFAuth.Visibility = Visibility.Visible;
                    loginButton.IsEnabled = true;
                    ProgressRing.Visibility = Visibility.Collapsed;
                    ProgressRing.IsActive = false;
                    LoginText.Visibility = Visibility.Visible;
                    if (result.SmsSupported)
                        MFAsms.Visibility = Visibility.Visible;
                    else
                        MFAsms.Visibility = Visibility.Collapsed;
                }
                else if (result.Token != null)
                {
                    App.LogIn();
                    App.SubpageClosed();
                }
            }
            else if(MFAuth.Visibility == Visibility.Visible)
            {
                //2FA AUTHENTICATION
                LoginResult result = new LoginResult();
                var mfapass = MFAPassword.Password;
                await Task.Run(async () =>
                {
                    if(sms)
                        result = await RESTCalls.LoginSMS(mfapass, mfaTicket);
                    else
                        result = await RESTCalls.LoginMFA(mfapass, mfaTicket);
                });
                if (result.exception != null)
                {
                    string ermsg = "";
                    switch (result.exception.Message)
                    {
                        case "Response status code does not indicate success: 400().":
                            ermsg = "Response code (from Discord Servers) indicates failure (400), please check your email and password";
                            break;
                        case "TBD":
                            ermsg = "A bug in the code is preventing log in, some imported code (refit) isn't working, trying again later may work";
                            break;
                        case "ILoginService doesn't look like a Refit interface. Make sure it has at least one method with a Refit HTTP method attribute and Refit is installed in the project.":
                            ermsg = "A bug in the code is preventing log in. Some imported code (Refit) isn't working as expected";
                            break;
                        default:
                            ermsg = "Unknown error, maybe try again later";
                            break;
                    }
                    MessageDialog msg = new MessageDialog(ermsg);
                    await msg.ShowAsync();
                }
                else if (result.Token != null)
                {
                    App.LogIn();
                    App.SubpageClosed();
                }
            }
            else if(TokenAuth.Visibility == Visibility.Visible)
            {
                using(HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Token.Password);
                    var resp = await client.GetAsync("https://discordapp.com/api/v6/users/@me");
                    
                    if (resp.IsSuccessStatusCode)
                    {
                        PasswordCredential credentials = new PasswordCredential("Token", "logintoken", Token.Password);
                        Storage.PasswordVault.Add(credentials);
                        App.LogIn();
                        App.SubpageClosed();
                    }
                    else
                    {
                        MessageDialog md = new MessageDialog("Sorry, but that token didn't work. Are you sure it was valid?", "Login failed");
                        await md.ShowAsync();
                    }
                }
                
            }
            loginButton.IsEnabled = true;
            ProgressRing.Visibility = Visibility.Collapsed;
            ProgressRing.IsActive = false;
            LoginText.Visibility = Visibility.Visible;

        }

        private async void Register(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discordapp.com/register"));
        }

        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discordapp.com/"));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as HyperlinkButton).ContextFlyout.ShowAt(sender as HyperlinkButton);
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            if(LoginWithToken.Tag.ToString() == "Token")
            {
                LoginWithToken.Content = "Go back";
                LoginWithToken.Tag = "Discord";
                MFAuth.Visibility = Visibility.Collapsed;
                NormalAuth.Visibility = Visibility.Collapsed;
                TokenAuth.Visibility = Visibility.Visible;
            }
            else if(LoginWithToken.Tag.ToString() == "Discord")
            {
                LoginWithToken.Content = "Login with Discord Token";
                LoginWithToken.Tag = "Token";
                MFAuth.Visibility = Visibility.Collapsed;
                TokenAuth.Visibility = Visibility.Collapsed;
                NormalAuth.Visibility = Visibility.Visible;
            }
        }

        private void Password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
                LogIn(null, null);
        }

        private void Username_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Tab) && Password.Visibility == Visibility.Visible)
                Password.Focus(FocusState.Keyboard);
        }

        private void Token_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                LogIn(null, null);
        }

        private void MFAPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                LogIn(null, null);
        }

        bool sms = false;
        private async void MFAsms_Click(object sender, RoutedEventArgs e)
        {
            (sender as HyperlinkButton).IsEnabled = false;
            (sender as HyperlinkButton).Content = "Sending sms...";
            var result = await RESTCalls.SendLoginSms(mfaTicket);
            (sender as HyperlinkButton).Content = "Re-send SMS?";
            (sender as HyperlinkButton).IsEnabled = true;
            if (result.PhoneNumber == null)
            {
                MessageDialog md = new MessageDialog("Failed to send sms!");
                await md.ShowAsync();
            }
            else
            {
                MFAPassword.Header = "Code sent to " + result.PhoneNumber+":";
                sms = true;
            }
        }

        private void Navigating(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri != new Uri("https://discordapp.com/login"))
            {
                Username.Text = cusername;
                Password.Password = cpassword;
                LogIn(loginButton, null);
            }
        }
    }
}
