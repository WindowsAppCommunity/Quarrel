// Quarrel © 2022

using Quarrel.Services.APIs.GitHubService.Models;
using Quarrel.ViewModels.SubPages.Meta;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Meta
{
    public sealed partial class CreditPage : UserControl
    {
        public CreditPage()
        {
            this.InitializeComponent();
        }

        CreditPageViewModel ViewModel => (CreditPageViewModel)DataContext;

        private async void DeveloperClicked(object sender, ItemClickEventArgs e)
        {
            BindableDeveloper? developer = (BindableDeveloper)e.ClickedItem;
            await Launcher.LaunchUriAsync(new Uri(developer.User.ProfilePageUrl));
        }

        private async void ContributorClicked(object sender, ItemClickEventArgs e)
        {
            BindableContributor contributor = (BindableContributor)e.ClickedItem;
            await Launcher.LaunchUriAsync(new Uri(contributor.Contributor.ProfilePageUrl));
        }
    }
}
