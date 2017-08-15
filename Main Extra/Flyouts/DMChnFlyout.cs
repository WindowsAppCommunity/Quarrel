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
            MenuFlyoutItem profile = new MenuFlyoutItem() {
                Text = App.Translate("Profile"),
                Icon = new SymbolIcon(Symbol.ContactInfo),
                Tag = dm.Raw.Users.FirstOrDefault()
            };
            profile.Click += OpenProfile;
            menu.Items.Add(profile);

            if (Session.Online)
            {
                MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();

                MenuFlyoutItem addFriend = new MenuFlyoutItem()
                {
                    Text = App.Translate("AddFriend"),
                    Icon = new SymbolIcon(Symbol.AddFriend),
                    Tag = dm.Raw.Users.FirstOrDefault().Id
                };
                addFriend.Click += RemoveFriend;

                MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
                {
                    Text = App.Translate("AcceptFriendRequest"),
                    Tag = dm.Raw.Users.FirstOrDefault().Id,
                    Icon = new SymbolIcon(Symbol.AddFriend)
                };
                acceptFriendRequest.Click += AddFriend;

                MenuFlyoutItem removeFriend = new MenuFlyoutItem() {
                    Text = App.Translate("RemoveFriend"),
                    Icon = new SymbolIcon(Symbol.ContactPresence),
                    Tag = dm.Raw.Users.FirstOrDefault().Id
                };
                removeFriend.Click += RemoveFriend;

                MenuFlyoutItem block = new MenuFlyoutItem() {
                    Text = App.Translate("Block"),
                    Icon = new SymbolIcon(Symbol.BlockContact),
                    Tag = dm.Raw.Users.FirstOrDefault().Id
                };
                block.Click += BlockUser;

                MenuFlyoutItem unBlock = new MenuFlyoutItem()
                {
                    Text = App.Translate("Unblock"),
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
                } else
                {
                    menu.Items.Add(addFriend);
                    menu.Items.Add(block);
                }
            }
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
            App.NavigateToProfile(((sender as MenuFlyoutItem).Tag as Nullable<SharedModels.User>).Value);
            //ShowUserDetails((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
