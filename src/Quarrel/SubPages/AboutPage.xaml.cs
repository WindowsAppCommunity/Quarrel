// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to display general information about the app.
    /// </summary>
    public sealed partial class AboutPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPage"/> class.
        /// </summary>
        public AboutPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the app's version number.
        /// </summary>
        public string AppVersion => string.Format(
            "{0}.{1}.{2}",
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Revision);

        /// <summary>
        /// Gets the last commit and branch used for build.
        /// </summary>
        public string CommitStatus => string.Format(
            "Commit {0} from {1}",
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch);

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 512;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

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
    }
}
