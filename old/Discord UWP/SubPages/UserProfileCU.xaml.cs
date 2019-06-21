using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DiscordAPI.API.Gateway;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.Managers;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class UserProfileCU : Page
    {
        public UserProfileCU()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private DiscordAPI.SharedModels.UserProfile profile;
        string userid;
        bool navFromFlyout = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SteamImage.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ConnectionLogos/steam" + (App.Current.RequestedTheme == ApplicationTheme.Dark ? "_light" : "_dark") + ".png"));

            usernameBox.Text = LocalState.CurrentUser.Username;
            emailBox.Text = LocalState.CurrentUser.Email ?? "";

            switch (LocalState.Settings.ExplicitContentFilter)
            {
                case 0:
                    Filtering2.IsChecked = true;
                    break;
                case 1:
                    Filtering1.IsChecked = true;
                    break;
                case 2:
                    Filtering2.IsChecked = true;
                    break;
            }

            //if (LocalState.Settings.FriendSourceFlag != null)
            //{
            //    FriendRequest0.IsChecked = LocalState.Settings.FriendSourceFlag.All;
            //    FriendRequest1.IsChecked = LocalState.Settings.FriendSourceFlag.MutualFriends;
            //    FriendRequest2.IsChecked = LocalState.Settings.FriendSourceFlag.MutualGuilds;
            //}

            if (string.IsNullOrEmpty(LocalState.CurrentUser.Avatar))
                deleteImage.Visibility = Visibility.Collapsed;
            else
                UserIcon.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + LocalState.CurrentUser.Id + "/" + LocalState.CurrentUser.Avatar + ".png?size=512"));
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }
      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToSettings();
        }


        string base64img = "";
        bool DeletedImage = false;
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
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

                    string uri = file.Path;

                    base64img = "data:" + file.ContentType + ";base64,";
                    // var tempfile = await RescaleImage(file, 128, 128);
                    base64img += Convert.ToBase64String(await filetobytes(file));
                    BitmapImage img = new BitmapImage();
                    UserIconRect.Opacity = 0;
                    using (var fileStream = await file.OpenStreamForReadAsync())
                    {
                        await img.SetSourceAsync(fileStream.AsRandomAccessStream());
                    }

                    UserIcon.ImageSource = img;
                    UserIconRect.Fade(1, 300).Start();
                    deleteImage.Content = "Cancel icon modification";
                    deleteImage.Visibility = Visibility.Visible;
                }
                catch { }
            }
        }

        private void CheckForSave()
        {
            if (usernameBox.Text.Length < 2 || usernameBox.Text.Length > 32 || emailBox.Text.Length == 0)
                SaveButton.IsEnabled = false;
            else
                SaveButton.IsEnabled = true;
        }

        private async Task<byte[]> filetobytes(Windows.Storage.StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
        private void deleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (deleteImage.Content.ToString() == "Cancel icon modification")
            {
                if (string.IsNullOrEmpty(LocalState.CurrentUser.Avatar))
                    UserIcon.ImageSource = null;
                else
                    UserIcon.ImageSource = new BitmapImage(Common.AvatarUri(LocalState.CurrentUser.Avatar, LocalState.CurrentUser.Id, "?size=512"));
                base64img = null;
                deleteImage.Content = "Delete";
                DeletedImage = false;
            }
            else
            {
                DeletedImage = true;
                UserIcon.ImageSource = null;
                deleteImage.Content = "Cancel icon modification";
            }
        }

        private async Task<StorageFile> RescaleImage(StorageFile sourceFile, uint width, uint height)
        {
            var imageStream = await sourceFile.OpenReadAsync();
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            StorageFile tempfile = await folder.CreateFileAsync("avatar.png", CreationCollisionOption.OpenIfExists);
            using (var resizedStream = await tempfile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;
                encoder.BitmapTransform.ScaledWidth = width;
                encoder.BitmapTransform.ScaledHeight = height;
                await encoder.FlushAsync();
            }
            return tempfile;
        }
        private void GuildIcon_ImageOpened(object sender, RoutedEventArgs e)
        {
            UserIconRect.Opacity = 0;
            UserIconRect.Fade(1, 300).Start();
        }

        private void usernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckForSave();
        }

        private void emailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckForSave();
        }

        private void changepass_Click(object sender, RoutedEventArgs e)
        {
            if(newpassword.Visibility == Visibility.Visible)
            {
                changepass.Content = "Change password";
                newpassword.Visibility = Visibility.Collapsed;
                newpassword.Opacity = 0;
                newpassword.Fade(1, 300).Start();
                newpassword.Password = "";
            }
            else
            {
                changepass.Content = "Cancel password change";
                newpassword.Visibility = Visibility.Visible;
                newpassword.Password = "";
            }
        }

        private async void MakeConnection(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(await RESTCalls.GetConnectionUrl(((sender as Button).Tag.ToString()))));
        }

        //private void FriendRequest0_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (FriendRequest0.IsChecked == true)
        //    {
        //        FriendRequest1.IsChecked = true;
        //        FriendRequest2.IsChecked = true;
        //    }
        //}
        //private void FriendRequest_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (FriendRequest0.IsChecked == true)
        //    {
        //        FriendRequest1.IsChecked = true;
        //        FriendRequest2.IsChecked = true;
        //    }
        //}


        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            switch (Pivot.SelectedIndex)
            {
                case 0:
                    UpdateUser();
                    break;
                case 1:
                    SaveUserSettings();
                    break;
            }
        }

        private async void UpdateUser()
        {
            saveBTNtext.Opacity = 0;
            SaveButton.IsEnabled = false;
            saveBTNprog.Visibility = Visibility.Visible;
            DiscordAPI.API.User.Models.ModifyUser modifyuser;
            if (!String.IsNullOrEmpty(password.Password))
            {
                string newpass = null;
                if (string.IsNullOrWhiteSpace(newpassword.Password))
                    newpass = null;
                if (string.IsNullOrEmpty(base64img))
                    modifyuser = new DiscordAPI.API.User.Models.ModifyUser() { Username = usernameBox.Text, Password = password.Password, NewPassword = newpass };
                else
                    modifyuser = new DiscordAPI.API.User.Models.ModifyUserAndAvatar() { Username = usernameBox.Text, Password = password.Password, Avatar = base64img, NewPassword = newpass };
                if (DeletedImage)
                    modifyuser = new DiscordAPI.API.User.Models.ModifyUserAndAvatar() { Username = usernameBox.Text, Password = password.Password, Avatar = null, NewPassword = newpass };
                User response = null;

                await Task.Run(async () =>
                {
                    response = await RESTCalls.ModifyCurrentUser(modifyuser);
                });
                if (response == null || response?.Id == null)
                {
                    string error = App.GetString("ThereWasAnErrorClarify");
                    if (!string.IsNullOrEmpty(response.Username))
                        error += response.Username + "\n";
                    if (!string.IsNullOrEmpty(response.Avatar))
                        error += response.Username + "\n";
                    if (!string.IsNullOrEmpty(response.Email))
                        error += response.Email + "\n";

                    MessageDialog md = new MessageDialog(error, App.GetString("/Dialogs/Sorry"));
                    saveBTNtext.Opacity = 1;
                    SaveButton.IsEnabled = true;
                    saveBTNprog.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CloseButton_Click(null, null);
                    LocalState.CurrentUser = response;
                }
            }
            else
            {
                //TODO: Highlight Current Password
                MustFill.Visibility = Visibility.Visible;
                saveBTNtext.Opacity = 1;
                SaveButton.IsEnabled = true;
                saveBTNprog.Visibility = Visibility.Collapsed;
            }
        }

        private async void SaveUserSettings()
        {
            var modify = new DiscordAPI.API.User.Models.ModifyUserSettings(LocalState.Settings);

            if (Filtering2.IsChecked == true)
            {
                modify.ExplicitContentFilter = 2;
            }
            else if (Filtering1.IsChecked == true)
            {
                modify.ExplicitContentFilter = 1;
            }
            else
            {
                modify.ExplicitContentFilter = 0;
            }

            //modify.FriendSourceFlag.All = FriendRequest0.IsChecked == true;
            //modify.FriendSourceFlag.MutualFriends = FriendRequest1.IsChecked == true;
            //modify.FriendSourceFlag.MutualGuilds = FriendRequest2.IsChecked == true;

            LocalState.Settings = await RESTCalls.ModifyUserSettings(modify);

            CloseButton_Click(null, null);
        }

        private void UserIconRect_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(LocalState.CurrentUser.Avatar, LocalState.CurrentUser.Id), e.GetPosition(this));
            }
        }

        private void UserIconRect_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(LocalState.CurrentUser.Avatar, LocalState.CurrentUser.Id), e.GetPosition(this));
            }
        }
    }
}
