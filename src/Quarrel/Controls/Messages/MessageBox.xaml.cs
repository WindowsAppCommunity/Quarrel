using Quarrel.ViewModels;
using Quarrel.ViewModels.Controls.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages
{
    public sealed partial class MessageBox : UserControl
    {
        public MessageBox()
        {
            this.InitializeComponent();
            DataContext = new MessageBoxViewModel();
        }

        public MessageBoxViewModel ViewModel => DataContext as MessageBoxViewModel;

        /// <summary>
        /// Add Emoji to message
        /// </summary>
        private void EmojiPicker_EmojiPicked(object sender, ViewModels.Controls.Emoji e)
        {
            ViewModel.MessageText += e.Surrogate;
        }
    }
}
