// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Settings.GuildSettings.Pages
{
    public sealed partial class ModerationPage : UserControl
    {
        public ModerationPage()
        {
            this.InitializeComponent();
        }

        public ModerationPageViewModel ViewModel => (ModerationPageViewModel)DataContext;
    }
}
