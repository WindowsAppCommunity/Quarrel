using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class AboutPage : UserControl, IConstrainedSubPage
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private async void JoinServer(object sender, RoutedEventArgs e)
        {
            try
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().InviteService.AcceptInvite("wQmQgtq");
            }
            catch
            {
                // TODO: State failure
            }
        }

        private async void OpenFeedbackHub(object sender, RoutedEventArgs e)
        {
            await Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        }

        private void OpenLicenses(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("LicensesPage");
        }

        private void OpenDiscordStatus(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("DiscordStatusPage");
        }

        public string AppVersion => string.Format("{0}.{1}.{2}",
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Revision);

        public string CommitStatus => string.Format("Commit {0} from {1}",
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch);

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;

        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
