// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using myTube;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Analytics;
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
        private IAnalyticsService _analyticsService = null;
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPage"/> class.
        /// </summary>
        public AboutPage()
        {
            this.InitializeComponent();

            AnalyticsService.Log(Constants.Analytics.Events.OpenAbout);
        }

        /// <summary>
        /// Gets the app's version number.
        /// </summary>
        public string AppVersion => string.Format(
            "{0}.{1}.{2}",
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build);

        /// <summary>
        /// Gets the last commit and branch used for build.
        /// </summary>
        // TODO: Localization
        public string CommitStatus => string.Format(
            Helpers.Constants.Localization.GetLocalizedString("CommitFrom"),
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch);

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 512;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        private async void JoinServer(object sender, RoutedEventArgs e)
        {
            try
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().InviteService.AcceptInvite("wQmQgtq");
                AnalyticsService.Log(Constants.Analytics.Events.JoinedQuarrelServer);
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
            SubFrameNavigationService.NavigateTo("LicensesPage");
        }

        private void OpenDiscordStatus(object sender, RoutedEventArgs e)
        {
            SubFrameNavigationService.NavigateTo("DiscordStatusPage");
        }
    }
}
