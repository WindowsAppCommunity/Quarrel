using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Discord_UWP.CacheModels;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private MenuFlyout MakeTextChnMenu(GuildChannel chn)
        {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem editchannel = new MenuFlyoutItem() { Text = "Edit Channel", Tag = chn.Raw.Id };
            editchannel.Click += Editchannel;
            menu.Items.Add(editchannel);
            ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem() { Text = "Mute Channel", Tag = chn.Raw.Id };
            mute.IsChecked = Storage.MutedChannels.Contains(chn.Raw.Id);
            mute.Click += MuteChannel;
            menu.Items.Add(mute);
            return menu;
        }

        private void MuteChannel(object sender, RoutedEventArgs e)
        {
            if (Storage.MutedChannels.Contains((sender as ToggleMenuFlyoutItem).Tag.ToString()))
            {
                Storage.MutedChannels.Remove((sender as ToggleMenuFlyoutItem).Tag.ToString());
            } else
            {
                Storage.MutedChannels.Add((sender as ToggleMenuFlyoutItem).Tag.ToString());
            }
            Storage.SaveMutedChannels();
        }

        private void Editchannel(object sender, RoutedEventArgs e)
        {
            //TODO:
        }
    }
}
