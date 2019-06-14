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
        public static MenuFlyout MakeGuildMemberMenu(GuildMember member)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

            // Add "Profile" button
            MenuFlyoutItem profile = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Profile"),
                Tag = member.User,
                Icon = new SymbolIcon(Symbol.ContactInfo)
            };
            profile.Click += FlyoutManager.OpenProfile;
            menu.Items.Add(profile);

            // If member is not the current user
            if (member.User.Id != LocalState.CurrentUser.Id)
            {
                // Add "Message" button
                MenuFlyoutItem message = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/Message"),
                    Tag = member.User.Id,
                    Icon = new SymbolIcon(Symbol.Message)
                };
                message.Click += FlyoutManager.MessageUser;
                menu.Items.Add(message);
            }


            // Add Seperator
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);

            // If member is not the current user 
            if (member.User.Id != LocalState.CurrentUser.Id)
            {
                // Create "Invite to Server" subitem
                MenuFlyoutSubItem InviteToServer = new MenuFlyoutSubItem()
                {
                    Text = App.GetString("/Flyouts/InviteToServer")
                };

                // Add guilds with permission to list
                foreach (KeyValuePair<string, LocalModels.Guild> guild in LocalState.Guilds)
                {
                    // Administrator or has Create Instant Invite permissions
                    if (guild.Value.permissions.Administrator || guild.Value.permissions.CreateInstantInvite)
                    {
                        // Add Guild item
                        MenuFlyoutItem item = new MenuFlyoutItem() { Text = guild.Value.Raw.Name, Tag = new Tuple<string, string>(guild.Value.channels.FirstOrDefault().Value.raw.Id, member.User.Id) };
                        item.Click += FlyoutManager.InviteToServer;
                        InviteToServer.Items.Add(item);
                    }

                }

                // Add Invite to server
                menu.Items.Add(InviteToServer);
            }

            // Create "Add Friend" button
            MenuFlyoutItem addFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AddFriend"),
                Tag = member.User.Id,
                Icon = new SymbolIcon(Symbol.AddFriend)
            };
            addFriend.Click += FlyoutManager.AddFriend;

            // Create "Remove Friend" button
            MenuFlyoutItem removeFriend = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/RemoveFriend"),
                Tag = member.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            removeFriend.Click += FlyoutManager.RemoveFriend;

            // Create "Block" button
            MenuFlyoutItem block = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Block"),
                Tag = member.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.BlockContact)
            };
            block.Click += FlyoutManager.BlockUser;

            // Create "Unblock" button
            MenuFlyoutItem unBlock = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/Unblock"),
                Tag = member.User.Id,
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            unBlock.Click += FlyoutManager.RemoveFriend;

            // Create "Accept Friend Request" button
            MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
            {
                Text = App.GetString("/Flyouts/AcceptFriendRequest"),
                Tag = member.User.Id,
                Icon = new SymbolIcon(Symbol.AddFriend)
            };
            acceptFriendRequest.Click += FlyoutManager.AddFriend;

            // Choose buttons to add
            if (LocalState.Friends.ContainsKey(member.User.Id))
            {
                switch (LocalState.Friends[member.User.Id].Type)
                {
                    // No relation
                    case 0:
                        // Add "Add Friend" and "Block" buttons
                        menu.Items.Add(addFriend);
                        menu.Items.Add(block);
                        break;

                    // Friends
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
            // Member is current user
            else if (member.User.Id == LocalState.CurrentUser.Id)
            {
                // No buttons for current user
            }
            else
            {
                // Default to no relation
                menu.Items.Add(addFriend);
                menu.Items.Add(block);
            }


            // If member is not current user
            if (member.User.Id != LocalState.CurrentUser.Id)
            {
                // Add Separator
                MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
                menu.Items.Add(sep2);
            }

            // If can change user's nickname
            if (Permissions.CanChangeNickname(member.User.Id, App.CurrentGuildId))
            {
                // Add "Change Nickname" button
                MenuFlyoutItem changeNickname = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/ChangeNickname"),
                    Tag = member.User.Id,
                    Icon = new SymbolIcon(Symbol.Rename)
                };
                changeNickname.Click += FlyoutManager.ChangeNickname;
                menu.Items.Add(changeNickname);
            }

            // If Current User has manage roles permission
            if (LocalState.Guilds[App.CurrentGuildId].permissions.Administrator || LocalState.Guilds[App.CurrentGuildId].permissions.ManageRoles)
            {
                // Create "Roles" subitem
                MenuFlyoutSubItem roles = new MenuFlyoutSubItem()
                {
                    Text = App.GetString("/Flyouts/Roles")
                    //Tag = member.Raw.User.Id,
                    //Icon = new SymbolIcon(Symbol.)
                };

                // Add Server's roles to Role SubItem
                foreach (Role role in LocalState.CurrentGuild.roles.Values.OrderByDescending(x => x.Position))
                {
                    ToggleMenuFlyoutItem roleItem = new ToggleMenuFlyoutItem()
                    {
                        Text = role.Name,
                        Tag = new Tuple<string, string>(role.Id, member.User.Id),
                        Foreground = Common.IntToColor(role.Color),
                        IsChecked = member.Roles != null && member.Roles.Contains(role.Id),
                        //Style = (Style)App.Current.Resources["ToggleOnlyCheckbox"],
                        IsEnabled = LocalState.Guilds[App.CurrentGuildId].members[LocalState.CurrentUser.Id].Roles.FirstOrDefault() != null && role.Position < LocalState.Guilds[App.CurrentGuildId].roles[LocalState.Guilds[App.CurrentGuildId].members[LocalState.CurrentUser.Id].Roles.FirstOrDefault()].Position || LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId == LocalState.CurrentUser.Id  //TODO: Double check role system
                    };
                    roleItem.Click += FlyoutManager.AddRole;
                    if (role.Name != "@everyone")
                    {
                        roles.Items.Add(roleItem);
                    }
                }

                // Add Roles subitem to menu
                menu.Items.Add(roles);
            }

            // If Current User has kick members permission
            if ((LocalState.Guilds[App.CurrentGuildId].permissions.Administrator || LocalState.Guilds[App.CurrentGuildId].permissions.KickMembers) && LocalState.Guilds[App.CurrentGuildId].GetHighestRole(member.Roles).Position < LocalState.Guilds[App.CurrentGuildId].GetHighestRole(LocalState.Guilds[App.CurrentGuildId].members[LocalState.CurrentUser.Id].Roles).Position || LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId == LocalState.CurrentUser.Id && member.User.Id != LocalState.CurrentUser.Id)
            {
                // Add "Kick Member" button
                MenuFlyoutItem kickMember = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/KickMember"),
                    Tag = member.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.BlockContact)
                };
                kickMember.Click += FlyoutManager.KickMember;
                menu.Items.Add(kickMember);
            }
            // If Member is current user and current user is not owner
            else if (member.User.Id == LocalState.CurrentUser.Id && LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                // Add "Leave Server" button
                MenuFlyoutItem leaveServer = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/LeaveServer"),
                    Tag = member.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Remove)
                };
                leaveServer.Click += FlyoutManager.LeaveServer;
                menu.Items.Add(leaveServer);
            }

            // If User has ban members permission
            if (((LocalState.Guilds[App.CurrentGuildId].permissions.Administrator || LocalState.Guilds[App.CurrentGuildId].permissions.BanMembers) && LocalState.Guilds[App.CurrentGuildId].GetHighestRole(member.Roles).Position < LocalState.Guilds[App.CurrentGuildId].GetHighestRole(LocalState.Guilds[App.CurrentGuildId].members[LocalState.CurrentUser.Id].Roles).Position) || LocalState.Guilds[App.CurrentGuildId].Raw.OwnerId == LocalState.CurrentUser.Id && member.User.Id != LocalState.CurrentUser.Id)
            {
                // Add "Ban Member" button
                MenuFlyoutItem banMember = new MenuFlyoutItem()
                {
                    Text = App.GetString("/Flyouts/BanMember"),
                    Tag = member.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.BlockContact)
                };
                banMember.Click += FlyoutManager.BanMember;
                menu.Items.Add(banMember);
            }

            // If member is a bot
            if (member.User.Bot)
            {

                // Add Separator
                menu.Items.Add(new MenuFlyoutSeparator());

                // Add extra bot specific stuff
                foreach(var feature in BotExtrasManager.GetBotFeatures(member.User.Id))
                {
                    MenuFlyoutItem link = new MenuFlyoutItem()
                    {
                        Text = feature.Name,
                        Tag = feature.Url,
                        Icon = new FontIcon() { Glyph=feature.Icon }
                    };
                    link.Click += FlyoutManager.OpenURL;
                    menu.Items.Add(link);
                }
            }

            return menu;
        }
    }
}
