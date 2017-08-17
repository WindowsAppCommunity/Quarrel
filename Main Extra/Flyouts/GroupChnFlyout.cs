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
        private MenuFlyout MakeGroupChannelMenu(DmCache dm)
        {
            MenuFlyout menu = new MenuFlyout();
            int x = 0;
            foreach (SharedModels.User user in Storage.Cache.DMs[dm.Raw.Id].Raw.Users)
            {
                MenuFlyoutSubItem item = new MenuFlyoutSubItem()
                {
                    Text = user.Username
                };

                MenuFlyoutItem profile = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Profile"),
                    Icon = new SymbolIcon(Symbol.ContactInfo),
                    Tag = user
                };
                profile.Click += OpenProfile;

                item.Items.Add(profile);
                if (Session.Online)
                {
                    MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
                    item.Items.Add(sep1);

                    //if (dm.Raw.OwnerId == Storage.Cache.CurrentUser.Raw.Id)
                    //{
                    //    //Remove from group

                    //    MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
                    //    item.Items.Add(sep2);
                    //}

                    MenuFlyoutItem addFriend = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/AddFriend"),
                        Icon = new SymbolIcon(Symbol.AddFriend),
                        Tag = dm.Raw.Users.ToList()[x].Id
                    };
                    addFriend.Click += AddFriend;

                    MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                        Icon = new SymbolIcon(Symbol.AddFriend),
                        Tag = dm.Raw.Users.ToList()[x].Id
                    };
                    acceptFriendRequest.Click += AddFriend;

                    MenuFlyoutItem removeFriend = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/RemoveFriend"),
                        Icon = new SymbolIcon(Symbol.ContactPresence),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                        Tag = dm.Raw.Users.ToList()[x].Id
                    };
                    removeFriend.Click += RemoveFriend;

                    MenuFlyoutItem block = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/Block"),
                        Icon = new SymbolIcon(Symbol.BlockContact),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                        Tag = dm.Raw.Users.ToList()[x].Id
                    };
                    block.Click += BlockUser;

                    MenuFlyoutItem unBlock = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/Unblock"),
                        Icon = new SymbolIcon(Symbol.ContactPresence),
                        Tag = dm.Raw.Users.ToList()[x].Id
                    };
                    unBlock.Click += RemoveFriendClick;

                    if (Storage.Cache.Friends.ContainsKey(dm.Raw.Users.FirstOrDefault().Id))
                    {
                        switch (Storage.Cache.Friends[dm.Raw.Users.FirstOrDefault().Id].Raw.Type)
                        {
                            case 0:
                                item.Items.Add(addFriend);
                                item.Items.Add(block);
                                break;
                            case 1:
                                item.Items.Add(removeFriend);
                                item.Items.Add(block);
                                break;
                            case 2:
                                item.Items.Add(unBlock);
                                break;
                            case 3:
                                item.Items.Add(acceptFriendRequest);
                                item.Items.Add(block);
                                break;
                            case 4:
                                menu.Items.Add(block);
                                break;
                        }
                    }
                    else
                    {
                        item.Items.Add(addFriend);
                        item.Items.Add(block);
                    }
                }
                menu.Items.Add(item);
                x++;
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
    }
}
