// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Settings.UserSettings.Pages
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
