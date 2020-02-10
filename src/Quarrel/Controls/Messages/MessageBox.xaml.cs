using Quarrel.ViewModels.Controls.Messages;
using Refit;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
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

        #region Methods

        #region UI Events

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
            AddAttachment(file);
        }

        /// <summary>
        /// Remove attachment from List
        /// </summary>
        private void RemoveAttachment(object sender, RoutedEventArgs e)
        {
            ViewModel.Attachments.Remove((sender as Button).DataContext as StreamPart);
        }

        private async void PasteFromClipboard(object sender, TextControlPasteEventArgs e)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.StorageItems))
            {
                var param = await dataPackageView.GetStorageItemsAsync();
                foreach (var file in param)
                    AddAttachment(file as StorageFile);
            }
            else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                var bmpStream = await dataPackageView.GetBitmapAsync();
                ViewModel.Attachments.Add(new StreamPart((await bmpStream.OpenReadAsync()).AsStream(), "file.png"));
            }
        }

        #endregion

        #region Attachments

        public async void AddAttachment(StorageFile file)
        {
            if (file == null)
            {
                return;
            }
            var stream = await file.OpenStreamForReadAsync();
            ViewModel.Attachments.Add(new StreamPart(stream, string.Format("{0}{1}", file.DisplayName, file.FileType), file.ContentType));
        }

        #endregion

        #endregion
    }
}
