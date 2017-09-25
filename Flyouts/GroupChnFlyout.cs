using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Discord_UWP.Managers;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeGroupChannelMenu(DirectMessageChannel dm)
        {
            MenuFlyout menu = new MenuFlyout();
            int x = 0;
            foreach (User user in LocalState.DMs[dm.Id].Users)
            {
                MenuFlyoutSubItem item = new MenuFlyoutSubItem()
                {
                    Text = user.Username
                };

                MenuFlyoutItem profile = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Profile"),
                    //Text = "Profile",
                    Icon = new SymbolIcon(Symbol.ContactInfo),
                    Tag = user
                };
                profile.Click += FlyoutManager.OpenProfile;

                item.Items.Add(profile);
                MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
                item.Items.Add(sep1);

                if (dm.OwnerId == LocalState.CurrentUser.Id)
                {
                    //Remove from group

                    MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
                    item.Items.Add(sep2);
                }

                MenuFlyoutItem addFriend = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/AddFriend"),
                    //Text = "Add Friend",
                    Icon = new SymbolIcon(Symbol.AddFriend),
                    Tag = dm.Users.ToList()[x].Id
                };
                addFriend.Click += FlyoutManager.AddFriend;

                MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                    //Text = "Accept Friend Request",
                    Icon = new SymbolIcon(Symbol.AddFriend),
                    Tag = dm.Users.ToList()[x].Id
                };
                acceptFriendRequest.Click += FlyoutManager.AddFriend;

                MenuFlyoutItem removeFriend = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/RemoveFriend"),
                    //Text = "Remove Friend",
                    Icon = new SymbolIcon(Symbol.ContactPresence),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Tag = dm.Users.ToList()[x].Id
                };
                removeFriend.Click += FlyoutManager.RemoveFriend;

                MenuFlyoutItem block = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Block"),
                    //Text = "Block",
                    Icon = new SymbolIcon(Symbol.BlockContact),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Tag = dm.Users.ToList()[x].Id
                };
                block.Click += FlyoutManager.BlockUser;

                MenuFlyoutItem unBlock = new MenuFlyoutItem()
                {
                    //Text = App.GetString("/Flyouts/Unblock"),
                    Text = "Unblock",
                    Icon = new SymbolIcon(Symbol.ContactPresence),
                    Tag = dm.Users.ToList()[x].Id
                };
                unBlock.Click += FlyoutManager.RemoveFriend;

                if (LocalState.Friends.ContainsKey(dm.Users.FirstOrDefault().Id))
                {
                    switch (LocalState.Friends[dm.Users.FirstOrDefault().Id].Type)
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
                menu.Items.Add(item);
                x++;
            }
            return menu;
        }
    }
}
