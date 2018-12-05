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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WhatsNew : Page
    {
        public WhatsNew()
        {
            this.InitializeComponent();
        }
        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
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
        DispatcherTimer timer = new DispatcherTimer();
        private async void joinDiscordUWPServer(object sender, RoutedEventArgs e)
        {
            JoinServer.IsHitTestVisible = false;
            JoinServerText.Text = App.GetString("/About/JoinWait");
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += TimerOnTick;
            try
            {
                await RESTCalls.AcceptInvite("wQmQgtq");
            }
            catch /*(Exception exception)*/
            {
                JoinServerText.Text = App.GetString("/About/JoinFail");
                timer.Start();
                return;
            }
            JoinServerText.Text = App.GetString("/About/JoinSucess");
            timer.Start();
        }

        private void TimerOnTick(object sender, object e)
        {
            timer.Tick -= TimerOnTick;
            timer.Stop();
            JoinServer.IsHitTestVisible = true;
            JoinServerText.Text = App.GetString("/About/JoinDiscordUWPServer");
        }

        //private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if(detailsBtn.Content.ToString() == "+ Show more details")
        //    {
        //        detailsBtn.Content = "- Show less details"; //TODO: Translate
        //        notDetailed.Visibility = Visibility.Collapsed;
        //        Detailed.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        detailsBtn.Content = "+ Show more details";
        //        notDetailed.Visibility = Visibility.Visible;
        //        Detailed.Visibility = Visibility.Collapsed;
        //    }
        //}

        private async void Paetron_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.patreon.com/QuarrelUWP"));
        }
    }
}
