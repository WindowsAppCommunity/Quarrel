// Copyright (c) Quarrel. All rights reserved.

using GitHubAPI.Models;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables.GitHub;
using Quarrel.ViewModels.SubPages;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for showing the developers and contributors list.
    /// </summary>
    public sealed partial class CreditPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditPage"/> class.
        /// </summary>
        public CreditPage()
        {
            this.InitializeComponent();
            DataContext = new CreditPageViewModel();
        }

        /// <summary>
        /// Gets the contributor information.
        /// </summary>
        public CreditPageViewModel ViewModel => DataContext as CreditPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 512;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        /// <summary>
        /// Open Contributor's GitHub page.
        /// </summary>
        private async void ContributorClicked(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((e.ClickedItem as Contributor).ProfilePageUrl));
        }

        /// <summary>
        /// Open Developer's GitHub page.
        /// </summary>
        private async void DeveloperClicked(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((e.ClickedItem as BindableDeveloper).Model.ProfilePageUrl));
        }
    }
}
