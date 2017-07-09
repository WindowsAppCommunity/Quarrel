using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LockScreen : Page
    {
        public LockScreen()
        {
            this.InitializeComponent();
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();
            var info = new ContinuumNavigationTransitionInfo();
            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ProgressRing.Visibility = Visibility.Visible;
            ProgressRing.IsActive = true;
            LoginText.Visibility = Visibility.Collapsed;
            try
            {
                string email = Email.Text;
                string password = Password.Password;
                await Task.Run(async () =>
                {
                    await Session.Login(email, password);
                    Session.Online = true;
                    Storage.Token = Session.Token;
                    Storage.SaveUser();
                });
                Frame.Navigate(typeof(Main));
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "An error occurred while sending the request":
                        await new MessageDialog("You're offline, try again when online").ShowAsync();
                        break;
                    case "Response status code does not indicate success: 400 ()":
                        await new MessageDialog("Error 400: There was as issue, if you haven't signed in from this device it's worth checking your email for an email from Discord and trying again").ShowAsync();
                        break;
                    case "Response status code does not indicate success: 401 ()":
                        await new MessageDialog("Error 401: There was an issue with interacting with the server, try again").ShowAsync();
                        break;
                    case "Response status code does not indicate success: 403 ()":
                        await new MessageDialog("Error 403: You are forbidden to perform this action, you likely just haven't signed in from this location, check your email to verify this signin location and try again").ShowAsync();
                        break;
                    case "Response status code does not indicate success: 429 ()":
                        await new MessageDialog("Error 429: You've been rate limited, try again in a few nanoseconds.").ShowAsync();
                        break;
                    case "Response status code does not indicate success: 500 ()":
                        await new MessageDialog("Error 500: There's an issue interacting with the server, and it's the server's fault. Try again later, Discord tends to fix server crashes fast.").ShowAsync();
                        break;
                    default:
                        await new MessageDialog("Error: " + ex.Message + "Try Again after checking your email to see if you need to verify this sign-in location").ShowAsync();
                        break;
                }
                (sender as Button).IsEnabled = true;
                ProgressRing.Visibility = Visibility.Collapsed;
                ProgressRing.IsActive = false;
                LoginText.Visibility = Visibility.Visible;
            }
        }
    }
}
