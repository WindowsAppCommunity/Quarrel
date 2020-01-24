using Quarrel.ViewModels;
using Quarrel.ViewModels.Models.Bindables;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages
{
    public sealed partial class MessageFlyout : UserControl
    {
        public MessageFlyout()
        {
            this.InitializeComponent();
        }

        public BindableMessage Message => DataContext as BindableMessage;
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

    }
}
