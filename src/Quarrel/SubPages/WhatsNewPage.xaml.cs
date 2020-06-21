// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to display features added in the most recent update.
    /// </summary>
    public sealed partial class WhatsNewPage : UserControl, IConstrainedSubPage
    {
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhatsNewPage"/> class.
        /// </summary>
        public WhatsNewPage()
        {
            this.InitializeComponent();
            SimpleIoc.Default.GetInstance<IAnalyticsService>().Log(Constants.Analytics.Events.OpenWhatsNew);
        }

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 384;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        /// <summary>
        /// Gets a value indicating whether or not to show the insider label.
        /// </summary>
        public bool IsInsider => App.IsInsiderBuild;

        /// <summary>
        /// Gets the app's version number.
        /// </summary>
        public string AppVersion => string.Format(
            "{0}.{1}.{2}",
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build);

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        private void Close(object sender, RoutedEventArgs e)
        {
            SubFrameNavigationService.GoBack();
        }

        private async void JoinQuarrelServer(object sender, RoutedEventArgs e)
        {
            try
            {
                await SimpleIoc.Default.GetInstance<IDiscordService>().InviteService.AcceptInvite("wQmQgtq");
                SimpleIoc.Default.GetInstance<IAnalyticsService>().Log(Constants.Analytics.Events.JoinedQuarrelServer);
            }
            catch
            {
                // TODO: State failure
            }
        }
    }
}
