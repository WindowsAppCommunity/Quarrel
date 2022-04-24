// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings
{
    public sealed partial class UserSettingsPage : UserControl
    {
        public UserSettingsPage()
        {
            this.InitializeComponent();
        }

        UserSettingsPageViewModel ViewModel => (UserSettingsPageViewModel)DataContext;
    }
}
