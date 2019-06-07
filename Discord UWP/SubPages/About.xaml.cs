using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Quarrel.Managers;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class About : Page
    {
        private DispatcherTimer statsTimer = new DispatcherTimer();
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
            if (version.Build != 0 || App.isDebug)
            {
                buildNumber.Visibility = Visibility.Visible;
                buildId.Visibility = Visibility.Visible;
            }

            statsTimer.Tick += StatsTimer_Tick;
            statsTimer.Interval = TimeSpan.FromSeconds(0.5);
            statsTimer.Start();
            UpdateStats();
        }

        private void StatsTimer_Tick(object sender, object e)
        {
           UpdateStats();
            statsTimer.Start();
        }

        private void UpdateStats()
        {
            var stats = GatewayManager.Gateway.GetStats();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StorageFile commitinfo = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/CommitInfo.txt"));
            IList<string> details = await FileIO.ReadLinesAsync(commitinfo);
            buildId.Text = App.GetString("/About/Commit") + " " + details[0] + ", " + Common.HumanizeDate(DateTime.Parse(details[1]), null);
            buildNumber.Text = App.GetString("/About/Build") + " " + details[2];

            try
            {
                List<string> purchaseList = new List<string>() { "RemoveAds", "Remove Ads", "Polite Dontation", "SignificantDontation", "OMGTHXDonation", "RidiculousDonation" };
                var licenseInformation = CurrentApp.LicenseInformation;
                foreach (string item in purchaseList)
                {
                    if (licenseInformation.ProductLicenses[item].IsActive)
                    {
                        if (PuracheList.Text == "")
                        {
                            PuracheList.Text += item;
                        }
                        else
                        {
                            PuracheList.Text += " ," + item;
                        }
                    }
                }
                if (PuracheList.Text == "")
                {
                    PuracheList.Text = "None";
                }
            }
            catch
            {
                if (PuracheList.Text == "")
                {
                    PuracheList.Text = "None (errors loading)";
                }
            }
        }
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
            if(timer.IsEnabled) timer.Stop();
            if(statsTimer.IsEnabled) statsTimer.Stop();
            timer.Tick -= TimerOnTick;
            statsTimer.Tick -= StatsTimer_Tick;
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

        private void TimerOnTick(object sender, object o)
        {
            timer.Tick -= TimerOnTick;
            timer.Stop();
            JoinServer.IsHitTestVisible = true;
            JoinServerText.Text = App.GetString("/About/JoinDiscordUWPServerTB.Text");
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubPages.Licenses));
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubPages.DiscordStatus));
        }
    }
}
