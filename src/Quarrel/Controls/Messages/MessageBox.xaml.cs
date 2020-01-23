using Quarrel.ViewModels;
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
        }

        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Add Emoji to message
        /// </summary>
        private void EmojiPicker_EmojiPicked(object sender, ViewModels.Controls.Emoji e)
        {
            ViewModel.MessageText += e.Surrogates;
        }
    }
}
