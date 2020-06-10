// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels;
using Quarrel.ViewModels.Models.Suggesitons;
using Refit;
using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Messages
{
    /// <summary>
    /// Control for drafting messages.
    /// </summary>
    public sealed partial class MessageBox : UserControl
    {
        private Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        public MessageBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets message drafting data.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Adds a file's data to the attachment list.
        /// </summary>
        /// <param name="file">The file being attached.</param>
        public async void AddAttachment(StorageFile file)
        {
            if (file == null)
            {
                return;
            }

            var stream = await file.OpenStreamForReadAsync();
            ViewModel.Attachments.Add(new StreamPart(stream, string.Format("{0}{1}", file.DisplayName, file.FileType), file.ContentType));
        }

        /// <summary>
        /// Select file and add to attachment List.
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
        /// Remove attachment from List.
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
                {
                    AddAttachment(file as StorageFile);
                }
            }
            else if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                string hexValue = string.Empty;

                for (int i = 0; i < 8; i++)
                {
                    int num = random.Next(0, int.MaxValue);
                    hexValue += num.ToString("X8");
                }

                var bmpDPV = await dataPackageView.GetBitmapAsync();
                string fileName = $"{hexValue}.png";
                var bmpSTR = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (var writeStream = (await bmpSTR.OpenStreamForWriteAsync()).AsRandomAccessStream())
                using (var readStream = await bmpDPV.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(readStream.CloneStream());
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, writeStream);
                    encoder.SetSoftwareBitmap(await decoder.GetSoftwareBitmapAsync());
                    await encoder.FlushAsync();
                    ViewModel.Attachments.Add(new StreamPart(await bmpSTR.OpenStreamForReadAsync(), fileName, bmpSTR.ContentType));
                }
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            popup.VerticalOffset = -e.NewSize.Height;
        }

        private void MessageEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SuggestionList.Width = e.NewSize.Width;
        }

        private void MessageEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void MessageEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void SuggestionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectSuggestion(e.ClickedItem as ISuggestion);
        }
    }
}
