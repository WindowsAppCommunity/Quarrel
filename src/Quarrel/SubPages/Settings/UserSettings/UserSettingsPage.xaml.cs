// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.UserSettings;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Settings.UserSettings
{
    public sealed partial class UserSettingsPage : UserControl
    {
        public UserSettingsPage()
        {
            this.InitializeComponent();
        }

        public UserSettingsPageViewModel ViewModel => (UserSettingsPageViewModel)DataContext;
    }
}
