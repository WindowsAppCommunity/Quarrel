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
            return menu;
        }

        private void Editchannel(object sender, RoutedEventArgs e)
        {
            //TODO:
        }
    }
}
