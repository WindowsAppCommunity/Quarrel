using GitHubAPI.Models;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables.GitHub;
using Quarrel.ViewModels.SubPages;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    public sealed partial class CreditPage : UserControl, IConstrainedSubPage
    {
        public CreditPage()
        {
            this.InitializeComponent();
            DataContext = new CreditPageViewModel();
        }

        public CreditPageViewModel ViewModel => DataContext as CreditPageViewModel;

        #region Methods

        /// <summary>
        /// Open Contributor's GitHub page
        /// </summary>
        private async void ContributorClicked(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((e.ClickedItem as Contributor).ProfilePageUrl));
        }

        /// <summary>
        /// Open Developer's GitHub page
        /// </summary>
        private async void DeveloperClicked(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((e.ClickedItem as BindableDeveloper).Model.ProfilePageUrl));
        }

        #endregion

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;

        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
