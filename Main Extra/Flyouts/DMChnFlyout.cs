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
            MenuFlyoutItem profile = new MenuFlyoutItem() { Text = App.GetString("/Flyouts/Profile"), Tag = dm.Raw.Users.FirstOrDefault() };
            profile.Click += OpenProfile;
            menu.Items.Add(profile);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            MenuFlyoutItem removeFriend = new MenuFlyoutItem() { Text = App.GetString("/Flyouts/RemoveFriend"), Tag = dm.Raw.Users.FirstOrDefault().Id };
            removeFriend.Click += RemoveFriend;
            menu.Items.Add(removeFriend);
            MenuFlyoutItem addFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AddFriend"),
                Icon = new SymbolIcon(Symbol.AddFriend),
                Tag = dm.Raw.Users.FirstOrDefault().Id
            };
            addFriend.Click += AddFriend;
            MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                Tag = dm.Raw.Users.FirstOrDefault().Id,
                Icon = new SymbolIcon(Symbol.AddFriend)
            };
            acceptFriendRequest.Click += AddFriend;
            MenuFlyoutItem block = new MenuFlyoutItem() { Text = App.GetString("/Flyouts/Block"), Tag = dm.Raw.Users.FirstOrDefault().Id };
            block.Click += BlockUser;
            menu.Items.Add(block);

            MenuFlyoutItem unBlock = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Unblock"),
                Tag = dm.Raw.Users.FirstOrDefault().Id,
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            unBlock.Click += RemoveFriendClick;

            if (Storage.Cache.Friends.ContainsKey(dm.Raw.Users.FirstOrDefault().Id))
            {
                switch (Storage.Cache.Friends[dm.Raw.Users.FirstOrDefault().Id].Raw.Type)
                {
                    case 0:
                        menu.Items.Add(addFriend);
                        menu.Items.Add(block);
                        break;
                    case 1:
                        menu.Items.Add(removeFriend);
                        menu.Items.Add(block);
                        break;
                    case 2:
                        menu.Items.Add(unBlock);
                        break;
                    case 3:
                        menu.Items.Add(acceptFriendRequest);
                        menu.Items.Add(block);
                        break;
                    case 4:
                        menu.Items.Add(block);
                        break;
                }
            }
            else
            {
                menu.Items.Add(addFriend);
                menu.Items.Add(block);
            }

            return menu;
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile(((sender as MenuFlyoutItem).Tag as Nullable<SharedModels.User>).Value);
            //ShowUserDetails((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
