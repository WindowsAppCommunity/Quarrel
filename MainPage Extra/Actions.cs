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
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /*private void EditMessage(object sender, RoutedEventArgs e)
       {
           SharedModels.Message msg  = User.GetMessage(((TextChannels.SelectedItem as ListViewItem).Tag as Nullable<SharedModels.GuildChannel>).Value.Id, (sender as Button).Tag.ToString());   
       }*/

        private void DeleteThisMessage(object sender, RoutedEventArgs e)
        {
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
            {
                Session.DeleteMessage((DMs.SelectedItem as ListViewItem).Tag.ToString(), (sender as Button).Tag.ToString());
                LoadChannelMessages(null, null);
            }
            else
            {
                Session.DeleteMessage(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id, (sender as Button).Tag.ToString());
                LoadChannelMessages(null, null);
            }
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
            {
                MessageBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string txt);
                Session.CreateMessage((DMs.SelectedItem as ListViewItem).Tag.ToString(), txt);
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");
            }
            else
            {
                MessageBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string txt);
                StringBuilder msg = new StringBuilder(txt);
                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                {
                    msg.Replace("@" + member.Value.Raw.User.Username + "#" + member.Value.Raw.User.Discriminator, "<@" + member.Value.Raw.User.Id + ">");
                }
                Session.CreateMessage(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id, msg.ToString());
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");
            }
        }

        private void EditMessage(object sender, RoutedEventArgs e)
        {
            if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
            {
                Session.EditMessage((DMs.SelectedItem as ListViewItem).Tag.ToString(), ((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2);
                LoadChannelMessages(null, null);
            }
            else
            {
                Session.EditMessage(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id, ((sender as Button).Tag as Tuple<string, string>).Item1, ((sender as Button).Tag as Tuple<string, string>).Item2);
                LoadChannelMessages(null, null);
            }
        }

        private async void TypingStarted(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            try
            {
                sender.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string text);
                if (text == "")
                {
                    SendBox.IsEnabled = false;
                }
                else
                {
                    SendBox.IsEnabled = true;
                }

                if (ServerList.SelectedIndex == 0)
                {
                    await Session.TriggerTypingIndicator((DMs.SelectedItem as ListViewItem).Tag.ToString());
                }
                else
                {
                    await Session.TriggerTypingIndicator(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);
                }
            }
            catch
            {

            }
        }

        private void AddServer(object sender, RoutedEventArgs e)
        {

        }

        private void AddChannel(object sender, RoutedEventArgs e)
        {
            CreateChannel.Visibility = Visibility.Visible;
            CreateChannelName.Text = "";
        }

        private void SaveChannelCreate(object sender, RoutedEventArgs e)
        {
            CreateChannel.Visibility = Visibility.Collapsed;
            TextChannels.SelectedIndex = TextChannels.Items.Count - 2;
            Session.CreateChannel((ServerList.SelectedItem as ListViewItem).Tag.ToString(), CreateChannelName.Text);
        }

        private void DeleteChannel(object sender, RoutedEventArgs e)
        {
            ChannelSettings.Visibility = Visibility.Collapsed;
            Session.DeleteChannel(_settingsChannel);
        }

        public void ToggleLoading(bool @on)
        {
            if (@on)
            {
                //Loading_Backdrop.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
            } else
            {
                //Loading_Backdrop.Visibility = Visibility.Collapsed;
                LoadingRing.IsActive = false;
            }
        }

        private void ToggleReaction(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == false) //Inverted since it changed
            {
                Session.DeleteReaction(((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item1, ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item2, ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji);

                if (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Me)
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Count - 1).ToString();
                } else
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Count).ToString();
                }
            } else
            {
                Session.CreateReaction(((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item1, ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item2, ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji);

                if (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Me)
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Count).ToString();
                }
                else
                {
                    (sender as ToggleButton).Content = ((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Emoji.Name + " " + (((sender as ToggleButton).Tag as Tuple<string, string, SharedModels.Reactions>).Item3.Count + 1).ToString();
                }
            }
        }
    }
}
