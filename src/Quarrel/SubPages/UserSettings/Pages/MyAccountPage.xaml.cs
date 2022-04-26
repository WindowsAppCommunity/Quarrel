// Quarrel © 2022

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    public sealed partial class MyAccountPage : UserControl
    {
        public MyAccountPage()
        {
            this.InitializeComponent();
        }

        public MyAccountPageViewModel ViewModel => (MyAccountPageViewModel)DataContext;
    }
}
