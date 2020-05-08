// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings Overview page.
    /// </summary>
    public sealed partial class OverviewSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewSettingsPage"/> class.
        /// </summary>
        public OverviewSettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the guild's Overview settings.
        /// </summary>
        public OverviewSettingsPageViewModel ViewModel => DataContext as OverviewSettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new OverviewSettingsPageViewModel(e.Parameter as BindableGuild);
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
                ViewModel.UpdateIcon("data:" + file.ContentType + ";base64," +
                    Convert.ToBase64String(await ImageParsing.FileToBytes(file)));
            }
        }
    }
}
