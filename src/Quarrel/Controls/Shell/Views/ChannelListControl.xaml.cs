using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell.Views
{
    public sealed partial class ChannelListControl : UserControl
    {
        public ChannelListControl()
        {
            this.InitializeComponent();
        }
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
