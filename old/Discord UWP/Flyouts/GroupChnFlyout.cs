using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;

namespace Quarrel.Flyouts
{
    partial class FlyoutCreator
    {
        public static MenuFlyout MakeGroupChannelMenu(DirectMessageChannel dm)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add users
            int x = 0;
            foreach (User user in LocalState.DMs[dm.Id].Users)
            {
                // Create SubItem for user
                MenuFlyoutSubItem item = new MenuFlyoutSubItem()
                {
                    Text = user.Username
                };

                // Add "Profile" button to subitem
                MenuFlyoutItem profile = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Profile"),
                    Icon = new SymbolIcon(Symbol.ContactInfo),
                    Tag = user
                };
                profile.Click += FlyoutManager.OpenProfile;
                item.Items.Add(profile);


                // Add Seperator
                MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
                item.Items.Add(sep1);

                // If current user owns the dm
                if (dm.OwnerId == LocalState.CurrentUser.Id)
                {
                    // Add "Remove from DM" button
                    MenuFlyoutItem removeFromDm = new MenuFlyoutItem()
                    {
                        Text = App.GetString("/Flyouts/RemoveFromGroup"),
                        Icon = new SymbolIcon(Symbol.LeaveChat),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                        Tag = new Tuple<string, string>(dm.Id, user.Id)
                    };
                    removeFromDm.Click += FlyoutManager.RemoveGroupUser;
                    item.Items.Add(removeFromDm);

                    // Add Seperator
                    MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
                    item.Items.Add(sep2);
                }

                // Create "Add Friend" button
                MenuFlyoutItem addFriend = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/AddFriend"),
                    Icon = new SymbolIcon(Symbol.AddFriend),
                    Tag = dm.Users.ToList()[x].Id
                };
                addFriend.Click += FlyoutManager.AddFriend;

                // Create "Accept Friend Request" button
                MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                    Icon = new SymbolIcon(Symbol.AddFriend),
                    Tag = dm.Users.ToList()[x].Id
                };
                acceptFriendRequest.Click += FlyoutManager.AddFriend;

                // Create "Remove Friend" button
                MenuFlyoutItem removeFriend = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/RemoveFriend"),
                    Icon = new SymbolIcon(Symbol.ContactPresence),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Tag = dm.Users.ToList()[x].Id
                };
                removeFriend.Click += FlyoutManager.RemoveFriend;

                // Create "Block" button
                MenuFlyoutItem block = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Block"),
                    Icon = new SymbolIcon(Symbol.BlockContact),
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Tag = dm.Users.ToList()[x].Id
                };
                block.Click += FlyoutManager.BlockUser;

                // Create "Unblock" button
                MenuFlyoutItem unBlock = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Unblock"),
                    Icon = new SymbolIcon(Symbol.ContactPresence),
                    Tag = dm.Users.ToList()[x].Id
                };
                unBlock.Click += FlyoutManager.RemoveFriend;

                // Choose buttons to add
                if (LocalState.Friends.ContainsKey(dm.Users.FirstOrDefault().Id))
                {
                    switch (LocalState.Friends[dm.Users.FirstOrDefault().Id].Type)
                    {
                        // No relation
                        case 0:
                            // Add "Add Friend" and "Block" buttons
                            item.Items.Add(addFriend);
                            item.Items.Add(block);
                            break;

                        // Friends
                        case 1:
                            // Add "Remove Friend" and "Block" buttons
                            item.Items.Add(removeFriend);
                            item.Items.Add(block);
                            break;

                        // Blocked
                        case 2:
                            // Add "Unblock" button
                            item.Items.Add(unBlock);
                            break;

                        // Incoming Friend Request
                        case 3:
                            // Add "Accept Friend Request" and "Block" buttons
                            item.Items.Add(acceptFriendRequest);
                            item.Items.Add(block);
                            break;

                        // Outgoing Friend Request
                        case 4:
                            // Add "Block" button
                            menu.Items.Add(block);
                            break;
                    }
                }
                else
                {
                    // Default to no relation
                    item.Items.Add(addFriend);
                    item.Items.Add(block);
                }
                
                // Add subitems
                menu.Items.Add(item);
                x++;
            }


            // Add seperator
            MenuFlyoutSeparator sep3 = new MenuFlyoutSeparator();
            menu.Items.Add(sep3);

            // Add "Leave DM" button
            MenuFlyoutItem leaveDm = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/LeaveGroup"),
                Icon = new SymbolIcon(Symbol.LeaveChat),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = new Tuple<string, string>(dm.Id, LocalState.CurrentUser.Id)
            };
            if (dm.OwnerId == LocalState.CurrentUser.Id)
            {
                leaveDm.Click += FlyoutManager.RemoveGroupUser;
            } else
            {
                leaveDm.Click += FlyoutManager.LeaveUnownedChannel;
            }
            menu.Items.Add(leaveDm);

            return menu;
        }
    }
}
