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
        /// <summary>
        /// Make Flyout for DMChannel
        /// </summary>
        /// <param name="dm">DM Channel</param>
        /// <returns>A MenuFlyout for the DM</returns>
        public static MenuFlyout MakeDMChannelMenu(DirectMessageChannel dm)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Profile" button
            MenuFlyoutItem profile = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Profile"),
                Icon = new SymbolIcon(Symbol.ContactInfo),
                Tag = dm.Users.FirstOrDefault()
            };
            profile.Click += FlyoutManager.OpenProfile;
            menu.Items.Add(profile);
            

            // Add seperator
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);


            // Create "Remove Friend" button
            MenuFlyoutItem removeFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/RemoveFriend"),
                Icon = new SymbolIcon(Symbol.ContactPresence),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = dm.Users.FirstOrDefault().Id
            };
            removeFriend.Click += FlyoutManager.RemoveFriend;

            // Create "Add Friend" button
            MenuFlyoutItem addFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AddFriend"),
                Icon = new SymbolIcon(Symbol.AddFriend),
                Tag = dm.Users.FirstOrDefault().Id
            };
            addFriend.Click += FlyoutManager.AddFriend;

            // Create "Accept Friend Request" button
            MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                Icon = new SymbolIcon(Symbol.AddFriend),
                Tag = dm.Users.FirstOrDefault().Id
            };
            acceptFriendRequest.Click += FlyoutManager.AddFriend;

            // Create "Block" button
            MenuFlyoutItem block = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Block"),
                Icon = new SymbolIcon(Symbol.BlockContact),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = dm.Users.FirstOrDefault().Id
            };
            block.Click += FlyoutManager.BlockUser;

            // Create "Unblock" button
            MenuFlyoutItem unBlock = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Unblock"),
                Tag = dm.Users.FirstOrDefault().Id,
                Icon = new SymbolIcon(Symbol.ContactPresence)
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
                        menu.Items.Add(addFriend);
                        menu.Items.Add(block);
                        break;

                    // Friend
                    case 1:
                        // Add "Remove Friend" and "Block" buttons
                        menu.Items.Add(removeFriend);
                        menu.Items.Add(block);
                        break;

                    // Blocked
                    case 2:
                        // Add "Unblock" button
                        menu.Items.Add(unBlock);
                        break;

                    // Incoming Friend Request
                    case 3:
                        // Add "Accept Friend Request" and "Block" buttons
                        menu.Items.Add(acceptFriendRequest);
                        menu.Items.Add(block);
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
                menu.Items.Add(addFriend);
                menu.Items.Add(block);
            }


            // Add Seperator
            MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
            menu.Items.Add(sep2);

            // Add "Close DM" button
            MenuFlyoutItem CloseDM = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/CloseDM"),
                Icon = new SymbolIcon(Symbol.LeaveChat),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = dm.Id,
            };
            CloseDM.Click += FlyoutManager.DeleteChannel;
            menu.Items.Add(CloseDM);

            return menu;
        }
    }
}
