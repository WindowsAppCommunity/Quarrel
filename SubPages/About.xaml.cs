using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            appVersion.Text = "Quarrel " + string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            var credentials = Storage.PasswordVault.FindAllByResource("Token");

            var creds = credentials[0];
            foreach (var cred in credentials)
                if (cred.UserName == Storage.Settings.DefaultAccount)
                    creds = cred;
            creds.RetrievePassword();
            token.Text = creds.Password;
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);

        //    if ((bool)e.Parameter)
        //    {
        //        Grid.SetRow(Header, 2);
        //    }
        //}

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private async void OpenFeedbackHub(object sender, RoutedEventArgs e)
        {
                var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                await launcher.LaunchAsync();
        }
        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
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
        DispatcherTimer timer = new DispatcherTimer();
        private async void joinDiscordUWPServer(object sender, RoutedEventArgs e)
        {
            JoinServer.IsHitTestVisible = false;
            JoinServerText.Text = App.GetString("/About/JoinWait");
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += TimerOnTick;
            try
            {
                await RESTCalls.AcceptInvite("wQmQgtq"); //TODO: Rig to App.Events
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

        private void TimerOnTick(object sender, object o)
        {
            timer.Tick -= TimerOnTick;
            timer.Stop();
            JoinServer.IsHitTestVisible = true;
            JoinServerText.Text = App.GetString("/About/JoinDiscordUWPServerTB.Text");
        }

    }
}
