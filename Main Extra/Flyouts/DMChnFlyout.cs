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
        private MenuFlyout MakeDMChannelMenu(DmCache dm)
        {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem profile = new MenuFlyoutItem() { Text = "Profile", Tag = dm.Raw.Users.FirstOrDefault().Id };
            profile.Click += OpenProfile;
            return menu;
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            ShowUserDetails((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
