// Quarrel © 2022

using Quarrel.ViewModels.SubPages.Settings.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Settings.UserSettings.Pages
{
    public sealed partial class DisplayPage : UserControl
    {
        public DisplayPage()
        {
            this.InitializeComponent();
        }

        public DisplayPageViewModel ViewModel => (DisplayPageViewModel)DataContext;
    }
}
