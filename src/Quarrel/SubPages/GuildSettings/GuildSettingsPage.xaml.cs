// Quarrel © 2022

using Quarrel.ViewModels.SubPages.GuildSettings;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.GuildSettings
{
    public sealed partial class GuildSettingsPage : UserControl
    {
        public GuildSettingsPage()
        {
            this.InitializeComponent();
        }

        public GuildSettingsPageViewModel ViewModel => (GuildSettingsPageViewModel)DataContext;
    }
}
