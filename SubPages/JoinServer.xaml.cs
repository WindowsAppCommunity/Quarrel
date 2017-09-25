using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JoinServer : Page
    {
        public JoinServer()
        {
            this.InitializeComponent();
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            try
            {
                Error.Visibility = Visibility.Collapsed;
                string InviteCode = Invite.Text;
                //Filter out the invite code from the link:
                InviteCode = InviteCode.Replace("https://discord.gg/", "");
                InviteCode = InviteCode.Replace("http://discord.gg/", "");
                InviteCode = InviteCode.Replace("https://discordapp.com/invite/", "");
                InviteCode = InviteCode.Replace("http://discordapp.com/invite/", "");
                await RESTCalls.AcceptInvite(Invite.Text); //TODO: Rig to App.Events
                CloseButton_Click(null, null);
            }
            catch
            {
                Error.Visibility = Visibility.Visible;
                SaveButton.IsEnabled = true;
            }
        }
    }
}
