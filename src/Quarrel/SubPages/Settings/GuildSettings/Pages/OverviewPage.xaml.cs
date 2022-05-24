// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Settings.GuildSettings.Pages
{
    public sealed partial class OverviewPage : UserControl
    {
        public OverviewPage()
        {
            this.InitializeComponent();
        }

        public OverviewPageViewModel ViewModel => (OverviewPageViewModel)DataContext;
    }
}
