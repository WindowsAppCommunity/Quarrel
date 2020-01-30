using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control to handle ChannelLsit
    /// </summary>
    public sealed partial class ChannelListControl : UserControl
    {
        public ChannelListControl()
        {
            this.InitializeComponent();
        }
        
        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
