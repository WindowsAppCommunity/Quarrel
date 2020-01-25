using GitHubAPI.Models;
using Quarrel.SubPages.Interfaces;
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


        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;

        public double MaxExpandedWidth { get; } = 512;

        #endregion

        /// <summary>
        /// Open Contributor's GitHub page
        /// </summary>
        private async void ContributorClicked(object sender, ItemClickEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri((e.ClickedItem as Contributor).ProfilePageUrl));
        }
    }
}
