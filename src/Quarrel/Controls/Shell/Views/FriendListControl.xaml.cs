using Quarrel.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    public sealed partial class FriendListControl : UserControl
    {
        public FriendListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
