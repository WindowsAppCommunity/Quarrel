// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings My Account page.
    /// </summary>
    public sealed partial class MyAccountSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountSettingsPage"/> class.
        /// </summary>
        public MyAccountSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new MyAccountSettingsViewModel();
        }

        /// <summary>
        /// Gets the user's basic account data.
        /// </summary>
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
                catch
                {
                }
            }
        }
    }
}
