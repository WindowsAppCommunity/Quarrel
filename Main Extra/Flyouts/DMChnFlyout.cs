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
            menu.Items.Add(profile);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            MenuFlyoutItem removeFriend = new MenuFlyoutItem() { Text = "Remove Friend", Tag = dm.Raw.Users.FirstOrDefault().Id };
            removeFriend.Click += RemoveFriend;
            menu.Items.Add(removeFriend);
            MenuFlyoutItem block = new MenuFlyoutItem() { Text = "Block", Tag = dm.Raw.Users.FirstOrDefault().Id };
            block.Click += BlockUser;
            menu.Items.Add(block);
            return menu;
        }

        private void BlockUser(object sender, RoutedEventArgs e)
        {
            Session.BlockUser((sender as MenuFlyoutItem).Tag.ToString()); //TODO: Confirm
        }

        private async void RemoveFriend(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.RemoveFriend((sender as MenuFlyoutItem).Tag.ToString());
            }); //TODO: Confirm
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile((sender as MenuFlyoutItem).Tag.ToString());
            //ShowUserDetails((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
