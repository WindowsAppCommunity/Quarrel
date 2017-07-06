using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.CacheModels;
using Discord_UWP.SharedModels;
#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private void DeleteThisMessage(object sender, RoutedEventArgs e)
        {
            if (App.CurrentGuildId == null)
            {
                Session.DeleteMessage(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id, (sender as Button).Tag.ToString());
                LoadDmChannelMessages(null, null);
            }
            else
            {
                Session.DeleteMessage(((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id, (sender as Button).Tag.ToString());
                LoadChannelMessages(null, null);
            }
        }

        private async void CreateMessage(object sender, RoutedEventArgs e)
        {
            string txt = MessageBox1.Text;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MessageBox1.Clear();
            });
            await Task.Run(() => Session.CreateMessage(App.CurrentChannelId, txt));
        }
        
        private void EditMessage(object sender, RoutedEventArgs e)
        {
            /*
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
            {
                Session.EditMessage(App.CurrentChannelId, ((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2);
            }
            else
            {
                Session.EditMessage(App.CurrentChannelId, ((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2);
            }
            */
        }

        private async void TypingStarted(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                await Task.Run(() => Session.TriggerTypingIndicator(App.CurrentChannelId));
            }
            catch
            {

            }
        }

        private void AddServer(object sender, RoutedEventArgs e)
        {

        }

        private void AddChannel(object sender, TappedRoutedEventArgs e)
        {
            CreateChannel.IsPaneOpen = true;
            CreateChannelName.Text = "";
        }

        private async void SaveChannelCreate(object sender, RoutedEventArgs e)
        {
            CreateChannel.Visibility = Visibility.Collapsed;
            TextChannels.SelectedIndex = TextChannels.Items.Count - 2;
            await Task.Run(() => Session.CreateChannel(App.CurrentGuildId, CreateChannelName.Text));
        }

        private void DeleteChannel(object sender, RoutedEventArgs e)
        {
            ChannelSettings.Visibility = Visibility.Collapsed;
            Session.DeleteChannel(_settingsPaneId);
            //await Task.Run(() => Session.DeleteChannel(_settingsPaneId));
        }

        private void ToggleReaction(object sender, RoutedEventArgs e)
        {
            /*
            if ((sender as ToggleButton).IsChecked == false) //Inverted since it changed
            {
                Session.DeleteReaction(((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item1, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item2, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji);

                if (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Me)
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Count - 1).ToString();
                }
                else
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Count).ToString();
                }
            }
            else
            {
                Session.CreateReaction(((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item1, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item2, ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji);

                if (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Me)
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Count).ToString();
                }
                else
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, Reactions>).Item3.Count + 1).ToString();
                }
            }
            */
        }

        #region OldCode
        //private async void AddAttachment(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        //    picker.FileTypeFilter.Add(".jpg");
        //    picker.FileTypeFilter.Add(".jpeg");
        //    picker.FileTypeFilter.Add(".png");

        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        using (Windows.Storage.Streams.IRandomAccessStream fileStream =
        //       await file.OpenAsync(FileAccessMode.Read))
        //        {
        //            // Set the image source to the selected bitmap.
        //            BitmapImage bitmapImage = new BitmapImage();
        //            bitmapImage.SetSource(fileStream);
        //            AttachmentImage.Height = 250;
        //            AttachmentImage.Source = bitmapImage;
        //            AttachmentImage.Tag = fileStream;
        //        }
        //    }
        //    else
        //    {
        //        MessageDialog msg = new MessageDialog("Couldn't attach");
        //        await msg.ShowAsync();
        //    }
        //}

        public void ToggleLoading(bool @on)
        {
            if (@on)
            {
                //Loading_Backdrop.Visibility = Visibility.Visible;
                //LoadingRing.IsActive = true;
            }
            else
            {
                //Loading_Backdrop.Visibility = Visibility.Collapsed;
                //LoadingRing.IsActive = false;
            }
        }
        #endregion
    }
}
