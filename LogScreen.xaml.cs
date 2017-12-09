using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private async void LogIn(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ProgressRing.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;
            LoginText.Visibility = Visibility.Collapsed;

            var result = await RESTCalls.Login(Username.Text, Password.Password);
            if (result == null)
            {
                PasswordCredential credentials = new PasswordCredential("LogIn", Username.Text, Password.Password);
                Storage.PasswordVault.Add(credentials);
                App.LoggingIn();
                App.SubpageClosed();
            } else
            {
                string ermsg = "";
                switch (result.Message)
                {
                    case "Response status code does not indicate success: 400().":
                        ermsg = "Response code (from Discord Servers) indicates failure (400), please check your email and password";
                        break;
                    case "TBD":
                        ermsg = "A bug in the code is preventing log in, some imported code (refit) isn't working, trying again later may work";
                        break;
                    default:
                        ermsg = "Unknown error, maybe try later";
                        break;
                }
                MessageDialog msg = new MessageDialog(ermsg);
                await msg.ShowAsync();

                (sender as Button).IsEnabled = true;
                ProgressRing.Visibility = Visibility.Collapsed;
                ProgressRing.IsActive = false;
                LoginText.Visibility = Visibility.Visible;
            }
        }

        private async void Register(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discordapp.com/register"));
        }

        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discordapp.com/"));
        }
    }
}
