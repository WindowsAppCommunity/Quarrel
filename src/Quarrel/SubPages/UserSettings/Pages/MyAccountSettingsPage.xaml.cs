using Quarrel.Helpers;
using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    public sealed partial class MyAccountSettingsPage : Page
    {
        public MyAccountSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new MyAccountSettingsViewModel();
        }

        public MyAccountSettingsViewModel ViewModel => this.DataContext as MyAccountSettingsViewModel;

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
                try
                {
                    ViewModel.AvatarUrl = file.Path;
                    ViewModel.Base64Avatar = "data:" + file.ContentType + ";base64," +
                        Convert.ToBase64String(await ImageParsing.FileToBytes(file));
                }
                // Mainly for rate limit
                catch { }
            }
        }
    }
}
