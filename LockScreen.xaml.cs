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
                await Session.Login(Email.Text, Password.Password);
                Storage.Token = Session.Token;
                Storage.SaveUser();
                this.Frame.Navigate(typeof(Main));
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog("Error: " + ex.Message + " Try Again after checking your email to see if you need to verify this sign-in location");
                await msg.ShowAsync();
                (sender as Button).IsEnabled = true;
                ProgressRing.Visibility = Visibility.Collapsed;
                ProgressRing.IsActive = false;
                LoginText.Visibility = Visibility.Visible;
            }
        }
    }
}
