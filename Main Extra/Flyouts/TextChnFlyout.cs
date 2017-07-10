using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Discord_UWP.CacheModels;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private MenuFlyout MakeTextChnMenu(GuildChannel chn)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            MenuFlyoutItem editchannel = new MenuFlyoutItem()
            {
                Text = "Edit Channel",
                Tag = chn.Raw.Id,
                Icon = new SymbolIcon(Symbol.Edit),
                Margin=new Thickness(-26,0,0,0)
            };
            editchannel.Click += Editchannel;
            menu.Items.Add(editchannel);
            ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem()
            {
                Text = "Mute Channel",
                Tag = chn.Raw.Id,
                Icon = new SymbolIcon(Symbol.Mute),
                Margin = new Thickness(-26, 0, 0, 0)
            };
            mute.IsChecked = Storage.MutedChannels.Contains(chn.Raw.Id);
            mute.Click += MuteChannel;
            menu.Items.Add(mute);
            MenuFlyoutItem markasread = new MenuFlyoutItem()
            {
                Text = "Mark as read",
                Tag = chn.Raw.Id,
                Icon = new SymbolIcon(Symbol.View),
                Margin = new Thickness(-26, 0, 0, 0),
                IsEnabled = !(TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == chn.Raw.Id) as SimpleChannel).IsUnread
            };
            menu.Items.Add(markasread);
            markasread.Click += MarkAsReadOnClick;
            MenuFlyoutItem deleteChannel = new MenuFlyoutItem()
            {
                Text = "Delete channel",
                Tag = chn.Raw.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.Delete),
                Margin = new Thickness(-26, 0, 0, 0),
                IsEnabled = !(TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == chn.Raw.Id) as SimpleChannel).IsUnread
            };
            menu.Items.Add(deleteChannel);
            deleteChannel.Click += DeleteChannelOnClick;
            return menu;
        }

        private void DeleteChannelOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Session.DeleteChannel((sender as MenuFlyoutItem).Tag.ToString());
        }

        private async void MarkAsReadOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var channelId = (sender as MenuFlyoutItem).Tag.ToString();
            await Task.Run(async () =>
            {
                await Session.AckMessage(channelId,
                    Storage.Cache.Guilds.FirstOrDefault(x => x.Value.Channels.ContainsKey(channelId))
                        .Value.Channels[channelId]
                        .Raw.LastMessageId);
            });
        }

        private void MuteChannel(object sender, RoutedEventArgs e)
        {
            if (Storage.MutedChannels.Contains((sender as ToggleMenuFlyoutItem).Tag.ToString()))
            {
                Storage.MutedChannels.Remove((sender as ToggleMenuFlyoutItem).Tag.ToString());
                (TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id == (sender as ToggleMenuFlyoutItem).Tag.ToString()) as SimpleChannel)
                    .IsMuted = false;
            } else
            {
                Storage.MutedChannels.Add((sender as ToggleMenuFlyoutItem).Tag.ToString());
                (TextChannels.Items.FirstOrDefault(x => (x as SimpleChannel).Id ==(sender as ToggleMenuFlyoutItem).Tag.ToString()) as SimpleChannel)
                    .IsMuted = true;
            }
            Storage.SaveMutedChannels();
        }

        private void Editchannel(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.EditChannel), (sender as MenuFlyoutItem).Tag as string);
            //TODO:
        }
    }
}
