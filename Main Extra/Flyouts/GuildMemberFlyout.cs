using Discord_UWP.API;
using Discord_UWP.API.Channel;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.API.Gateway;
using Discord_UWP.API.Guild;
using Discord_UWP.API.Login;
using Discord_UWP.API.Login.Models;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Discord_UWP.CacheModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Microsoft.Toolkit.Uwp;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private MenuFlyout MakeGuildMemberMenu(Member member)
        {
            MenuFlyout menu = new MenuFlyout();
            menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            MenuFlyoutItem profile = new MenuFlyoutItem()
            {
                Text = "Profile",
                Tag = member.Raw.User,
                Icon = new SymbolIcon(Symbol.ContactInfo)
            };
            profile.Click += gotoProfile;
            menu.Items.Add(profile);
            MenuFlyoutItem message = new MenuFlyoutItem()
            {
                Text = "Message",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.Message)
            };
            message.Click += MessageUser;
            menu.Items.Add(message);
            MenuFlyoutSeparator sep1 = new MenuFlyoutSeparator();
            menu.Items.Add(sep1);
            if (member.Raw.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                MenuFlyoutSubItem InviteToServer = new MenuFlyoutSubItem()
                {
                    Text = "Invite to Server"
                    //Tag = member.Raw.User.Id,
                    //Icon = new SymbolIcon(Symbol.)
                };
                foreach (KeyValuePair<string, Guild> guild in Storage.Cache.Guilds)
                {
                    if (guild.Value.perms.EffectivePerms.Administrator || guild.Value.perms.EffectivePerms.CreateInstantInvite)
                    {
                        MenuFlyoutItem item = new MenuFlyoutItem() { Text = guild.Value.RawGuild.Name, Tag = new Tuple<string, string>(guild.Value.Channels.FirstOrDefault().Value.Raw.Id, member.Raw.User.Id) };
                        item.Click += inviteToServer;
                        InviteToServer.Items.Add(item);
                    }

                }
                menu.Items.Add(InviteToServer);
            }
            MenuFlyoutItem addFriend = new MenuFlyoutItem()
            {
                Text = "Add Friend",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.AddFriend)
            };
            addFriend.Click += AddFriend;
            MenuFlyoutItem removeFriend = new MenuFlyoutItem()
            {
                Text = "Remove Friend",
                Tag = member.Raw.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            removeFriend.Click += RemoveFriendClick;
            MenuFlyoutItem block = new MenuFlyoutItem()
            {
                Text = "Block",
                Tag = member.Raw.User.Id,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                Icon = new SymbolIcon(Symbol.BlockContact)
            };
            block.Click += Block;
            MenuFlyoutItem unBlock = new MenuFlyoutItem()
            {
                Text = "Unblock",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.ContactPresence)
            };
            unBlock.Click += RemoveFriendClick;
            MenuFlyoutItem acceptFriendRequest = new MenuFlyoutItem()
            {
                Text = "Accept Friend Request",
                Tag = member.Raw.User.Id,
                Icon = new SymbolIcon(Symbol.AddFriend)
            };
            acceptFriendRequest.Click += AddFriend;
            if (Storage.Cache.Friends.ContainsKey(member.Raw.User.Id))
            {
                switch (Storage.Cache.Friends[member.Raw.User.Id].Raw.Type)
                {
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
            } else if (member.Raw.User.Id == Storage.Cache.CurrentUser.Raw.Id)
            {
                //None
            } else
            {
                menu.Items.Add(addFriend);
                menu.Items.Add(block);
            }
            if (member.Raw.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                MenuFlyoutSeparator sep2 = new MenuFlyoutSeparator();
                menu.Items.Add(sep2);
            }
            if ((member.Raw.User.Id == Storage.Cache.CurrentUser.Raw.Id && Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.ChangeNickname) || Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.ManageNicknames || Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.Administrator)
            {
                MenuFlyoutItem changeNickname = new MenuFlyoutItem()
                {
                    Text = "Change Nickname",
                    Tag = member.Raw.User.Id,
                    Icon = new SymbolIcon(Symbol.Edit)
                };
                changeNickname.Click += ChangeNickname;
                menu.Items.Add(changeNickname);
            }
            if (Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.Administrator || Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.ManageRoles)
            {
                MenuFlyoutSubItem roles = new MenuFlyoutSubItem()
                {
                    Text = "Roles"
                    //Tag = member.Raw.User.Id,
                    //Icon = new SymbolIcon(Symbol.)
                };

                foreach (SharedModels.Role role in Storage.Cache.Guilds[App.CurrentGuildId].Roles.Values.OrderByDescending(x => x.Position))
                {
                    ToggleMenuFlyoutItem roleItem = new ToggleMenuFlyoutItem()
                    {
                        Text = role.Name,
                        Tag = new Tuple<string, string>(role.Id, member.Raw.User.Id),
                        Foreground = Common.IntToColor(role.Color),
                        IsChecked = Storage.Cache.Guilds[App.CurrentGuildId].Members[member.Raw.User.Id].Raw.Roles.Contains(role.Id),
                        //Style = (Style)App.Current.Resources["ToggleOnlyCheckbox"],
                        IsEnabled = (role.Position < Storage.Cache.Guilds[App.CurrentGuildId].Roles[Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First()].Position || Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId == Storage.Cache.CurrentUser.Raw.Id)  //TODO: Double check role system
                    };
                    roleItem.Click += AddRole;
                    if (role.Name != "@everyone")
                    {
                        roles.Items.Add(roleItem);
                    }
                }
                menu.Items.Add(roles);
            }
            if (((Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.Administrator || Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.KickMembers) && member.MemberDisplayedRole.Position < Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id].MemberDisplayedRole.Position) || Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId == Storage.Cache.CurrentUser.Raw.Id && member.Raw.User.Id != Storage.Cache.CurrentUser.Raw.Id)
            {
                MenuFlyoutItem kickMember = new MenuFlyoutItem()
                {
                    Text = "Kick Member",
                    Tag = member.Raw.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.BlockContact)
                };
                kickMember.Click += KickMember;
                menu.Items.Add(kickMember);
            } else if (member.Raw.User.Id == Storage.Cache.CurrentUser.Raw.Id && Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
            {
                MenuFlyoutItem leaveServer = new MenuFlyoutItem()
                {
                    Text = "Leaver Server",
                    Tag = member.Raw.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.Remove)
                };
                leaveServer.Click += LeaveServer;
                menu.Items.Add(leaveServer);
            }
            if (Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.Administrator || Storage.Cache.Guilds[App.CurrentGuildId].perms.EffectivePerms.BanMembers)
            {
                MenuFlyoutItem banMember = new MenuFlyoutItem()
                {
                    Text = "Ban Member",
                    Tag = member.Raw.User.Id,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71)),
                    Icon = new SymbolIcon(Symbol.BlockContact)
                };
                banMember.Click += BanMember;
                menu.Items.Add(banMember);
            }
            if (false)
            {
                //TODO: style ToggleMenuFlyoutItem to have a checkbox on the right side
                ToggleMenuFlyoutItem mute = new ToggleMenuFlyoutItem()
                {
                    Text = "Mute",
                    Icon = new SymbolIcon(Symbol.Mute)
                };


                ToggleMenuFlyoutItem deafen = new ToggleMenuFlyoutItem()
                {
                    Text = "Deafen",
                    Icon = new SymbolIcon(Symbol.Mute)
                };

                MenuFlyoutSubItem moveChannel = new MenuFlyoutSubItem()
                {
                    Text = "Move Channel"
                };
            }
            return menu;
        }

        private void MessageUser(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void BanMember(object sender, RoutedEventArgs e)
        {
            Session.CreateBan(App.CurrentGuildId, (sender as MenuFlyoutItem).Tag.ToString(), new API.Guild.Models.CreateGuildBan() { DeleteMessageDays = 0});
            //TODO: Confirm+ban options
        }

        private void LeaveServer(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void KickMember(object sender, RoutedEventArgs e)
        {
            Session.RemoveGuildMember(App.CurrentGuildId, (sender as MenuFlyoutItem).Tag.ToString());
        }

        private void AddRole(object sender, RoutedEventArgs e)
        {
            Tuple<string, string> data = ((sender as ToggleMenuFlyoutItem).Tag as Tuple<string, string>);
            API.Guild.Models.ModifyGuildMember modify = new API.Guild.Models.ModifyGuildMember();
            List<string> roles = Storage.Cache.Guilds[App.CurrentGuildId].Members[data.Item2].Raw.Roles.ToList();
            if (roles.Contains(data.Item1))
            {
                roles.Remove(data.Item1);
            } else
            {
                roles.Add(data.Item1);
            }
            modify.Roles = roles.AsEnumerable();
            Session.ModifyGuildMember(App.CurrentGuildId, data.Item2, modify);
        }

        private void ChangeNickname(object sender, RoutedEventArgs e)
        {
            App.NavigateToNicknameEdit((sender as MenuFlyoutItem).Tag.ToString());
        }

        private async void Block(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.BlockUser((sender as MenuFlyoutItem).Tag.ToString());
            }); //TODO: Confirm
        }

        private async void RemoveFriendClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.RemoveFriend((sender as MenuFlyoutItem).Tag.ToString());
            }); //TODO: Confirm
        }

        private async void AddFriend(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Session.SendFriendRequest((sender as MenuFlyoutItem).Tag.ToString());
            });
        }

        private void inviteToServer(object sender, RoutedEventArgs e)
        {
            //TODO: Generate single use temp invite, send to selected user 
            Tuple<string, string> data = (sender as MenuFlyoutItem).Tag as Tuple<string, string>;
            Task.Run(async () => 
            {
                SharedModels.Invite invite = await Session.CreateInvite(data.Item1, new SharedModels.CreateInvite() { MaxUses = 1, Unique = true });
                //TODO: Make more effiecient
                foreach (KeyValuePair<string, DmCache> dm in Storage.Cache.DMs)
                {
                    if (dm.Value.Raw.Users.Count() == 1 && dm.Value.Raw.Users.FirstOrDefault().Id == data.Item2)
                    {
                        await Session.CreateMessage(dm.Key, "https://discord.gg/" + invite.String);
                        App.NavigateToGuildChannel(null, dm.Key);
                        return;
                    }
                }
            });
        }

        private void mentionUser(object sender, RoutedEventArgs e)
        {
            App.MentionUser((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void gotoProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile(((sender as MenuFlyoutItem).Tag as Nullable<SharedModels.User>).Value);
        }
    }
}
