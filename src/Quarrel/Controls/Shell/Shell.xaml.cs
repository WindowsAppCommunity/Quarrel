// Adam Dernis © 2022

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();
        }

        private void ToggleLeft(object sender, RoutedEventArgs e)
        {
            Drawer.ToggleLeft();
        }

        private void ToggleRight(object sender, RoutedEventArgs e)
        {
            Drawer.ToggleRight();
        }
    }
}
