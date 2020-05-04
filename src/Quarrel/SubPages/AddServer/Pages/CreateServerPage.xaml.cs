// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using Quarrel.ViewModels.SubPages.AddServer.Pages;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.SubPages.AddServer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateServerPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateServerPage"/> class.
        /// </summary>
        public CreateServerPage()
        {
            this.InitializeComponent();

            DataContext = new CreateServerPageViewModel();
        }

        /// <summary>
        /// Gets the new server data for creating a server.
        /// </summary>
        public CreateServerPageViewModel ViewModel => DataContext as CreateServerPageViewModel;

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;
        }

        private async void UploadAvatar(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                GuildIconImage.ImageSource = await LoadImage(file);
                ViewModel.UpdateIcon("data:" + file.ContentType + ";base64," +
                    Convert.ToBase64String(await ImageParsing.FileToBytes(file)));
            }
        }

        private void DeleteAvatar(object sender, RoutedEventArgs e)
        {
            GuildIconImage.ImageSource = null;
            ViewModel.UpdateIcon(null);
        }
    }
}
