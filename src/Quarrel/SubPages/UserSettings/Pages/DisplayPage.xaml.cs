// Quarrel © 2022

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
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
