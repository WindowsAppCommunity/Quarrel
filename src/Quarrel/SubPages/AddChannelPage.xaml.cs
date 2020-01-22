using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class AddChannelPage : UserControl, IConstrainedSubPage
    {

        public AddChannelPage()
        {
            this.InitializeComponent();
            this.DataContext = new AddChannelPageViewModel();
        }

        public AddChannelPageViewModel ViewModel => this.DataContext as AddChannelPageViewModel;


        public double MaxExpandedHeight { get; } = 300;

        public double MaxExpandedWidth { get; } = 400;

    }
}
