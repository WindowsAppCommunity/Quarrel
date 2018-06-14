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
        public static MenuFlyout MakeDMChannelMenu(DirectMessageChannel dm)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            MenuFlyoutItem profile = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Profile"),
                Icon = new SymbolIcon(Symbol.ContactInfo),
                Tag = dm.Users.FirstOrDefault()
            };
            profile.Click += FlyoutManager.OpenProfile;
            menu.Items.Add(profile);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            MenuFlyoutItem removeFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/RemoveFriend"),
                Icon = new SymbolIcon(Symbol.ContactPresence),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = dm.Users.FirstOrDefault().Id
            };
            removeFriend.Click += FlyoutManager.RemoveFriend;
            MenuFlyoutItem addFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AddFriend"),
                Icon = new SymbolIcon(Symbol.AddFriend),
                Tag = dm.Users.FirstOrDefault().Id
            };
            addFriend.Click += FlyoutManager.AddFriend;
            MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                Icon = new SymbolIcon(Symbol.AddFriend),
                Tag = dm.Users.FirstOrDefault().Id
            };
            acceptFriendRequest.Click += FlyoutManager.AddFriend;
            MenuFlyoutItem block = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Block"),
                Icon = new SymbolIcon(Symbol.BlockContact),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Tag = dm.Users.FirstOrDefault().Id
            };
            block.Click += FlyoutManager.BlockUser;

            MenuFlyoutItem unBlock = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Unblock"),
                Tag = dm.Users.FirstOrDefault().Id,
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            unBlock.Click += FlyoutManager.RemoveFriend;

            if (LocalState.Friends.ContainsKey(dm.Users.FirstOrDefault().Id))
            {
                switch (LocalState.Friends[dm.Users.FirstOrDefault().Id].Type)
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

            MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
            menu.Items.Add(sep2);

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
