using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP
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
        string mfaTicket;
        private async void LogIn(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ProgressRing.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;
            LoginText.Visibility = Visibility.Collapsed;

            if(NormalAuth.Visibility == Visibility.Visible)
            {
                //NORMAL AUTHENTICATION
                API.Login.Models.LoginResult result = new API.Login.Models.LoginResult();
                var username = Username.Text;
                var password = Password.Password;
                await Task.Run(async () =>
                {
                    result = await RESTCalls.Login(username, password);
                });
                if (result.exception != null)
                {
                    string ermsg = "";
                    switch (result.exception.Message)
                    {
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
                    (sender as Button).IsEnabled = true;
                    ProgressRing.Visibility = Visibility.Collapsed;
                    ProgressRing.IsActive = false;
                    LoginText.Visibility = Visibility.Visible;
                }
                else if (result.Token != null)
                {
                    
                    App.LoggingIn();
                    App.SubpageClosed();
                }
            }
            else if(MFAuth.Visibility == Visibility.Visible)
            {
                //2FA AUTHENTICATION
                API.Login.Models.LoginResult result = new API.Login.Models.LoginResult();
                var mfapass = MFAPassword.Password;
                await Task.Run(async () =>
                {
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
                    App.LoggingIn();
                    App.SubpageClosed();
                }
            }
            (sender as Button).IsEnabled = true;
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
    }
}
