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
            Password.PlaceholderText = App.Translate("Password");
            LoginText.Text = App.Translate("Login");
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
                    //Clear all data from Session
                    try
                    {
                        Session.RPC.Clear();
                        Session.PrecenseDict.Clear();
                        App.CurrentChannelId = null;
                        App.CurrentGuild = null;
                        App.GuildMembers.Clear();
                        App.Notes.Clear();
                    }
                    catch (Exception) { }

                    //Login
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
                        await new MessageDialog(App.GetString("/Main/OfflineError")).ShowAsync();
                        break;
                    case "Response status code does not indicate success: 400 ()":
                        await new MessageDialog(App.GetString("/Main/Error400")).ShowAsync();
                        break;
                    case "Response status code does not indicate success: 401 ()":
                        await new MessageDialog(App.GetString("/Main/Error401")).ShowAsync();
                        break;
                    case "Response status code does not indicate success: 403 ()":
                        await new MessageDialog(App.GetString("/Main/Error403")).ShowAsync();
                        break;
                    case "Response status code does not indicate success: 429 ()":
                        await new MessageDialog(App.GetString("/Main/Error429")).ShowAsync();
                        break;
                    case "Response status code does not indicate success: 500 ()":
                        await new MessageDialog(App.GetString("/Main/Error500")).ShowAsync();
                        break;
                    default:
                        await new MessageDialog(App.GetString("/Main/Error") + ": " + ex.Message + App.GetString("/Main/ErrorGeneric")).ShowAsync();
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
