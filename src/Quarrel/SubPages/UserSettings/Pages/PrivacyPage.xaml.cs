// Quarrel © 2022

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    public sealed partial class PrivacyPage : UserControl
    {
        public PrivacyPage()
        {
            this.InitializeComponent();
        }

        public PrivacyPageViewModel ViewModel => (PrivacyPageViewModel)DataContext;
    }
}
