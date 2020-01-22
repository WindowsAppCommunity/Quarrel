using Quarrel.Helpers;
using Quarrel.ViewModels.SubPages.Settings;
using Quarrel.ViewModels.SubPages.Settings.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.Settings.Pages
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
