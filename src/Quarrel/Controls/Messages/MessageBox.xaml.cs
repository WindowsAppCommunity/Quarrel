using Quarrel.ViewModels.Controls.Messages;
using Refit;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// Control for drafting messages
    /// </summary>
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

        /// <summary>
        /// Select file and add to attachment List
        /// </summary>
        private async void AddAttachment(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }
            var stream = await file.OpenStreamForReadAsync();
            ViewModel.Attachments.Add(new StreamPart(stream, string.Format("{0}{1}", file.DisplayName, file.FileType), file.ContentType)); 
        }

        /// <summary>
        /// Remove attachment from List
        /// </summary>
        private void RemoveAttachment(object sender, RoutedEventArgs e)
        {
            ViewModel.Attachments.Remove((sender as Button).DataContext as StreamPart);
        }
    }
}
