using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Discord_UWP.Controls;
using Discord_UWP.Flyouts;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Managers
{
    public class FlyoutManager
    {
        public enum Type { Guild, GuildMember, GroupMember, TextChn, DMChn, GroupChn}
        public static async Task<MenuFlyout> ShowMenu(Type type, string id, string parentId)
        {
            MenuFlyout flyout = new MenuFlyout();
            switch (type)
            {
                case Type.Guild:
                    if (id != "@me")
                    {
                        flyout = FlyoutCreator.MakeGuildMenu(LocalState.Guilds[id]);
                    }
                    break;
                case Type.DMChn:
                    flyout = FlyoutCreator.MakeDMChannelMenu(LocalState.DMs[id]);
                    break;
                case Type.GroupChn:
                    flyout = FlyoutCreator.MakeGroupChannelMenu(LocalState.DMs[id]);
                    break;
                case Type.TextChn:
                    flyout = FlyoutCreator.MakeTextChnMenu(LocalState.Guilds[parentId].channels[id]);
                    break;
                case Type.GuildMember:
                    if (parentId != null)
                    {
                        if (LocalState.Guilds[parentId].members.ContainsKey(id))
                        {
                            flyout = FlyoutCreator.MakeGuildMemberMenu(LocalState.Guilds[parentId].members[id]);
                        } else
                        {
                            flyout = FlyoutCreator.MakeGuildMemberMenu(await RESTCalls.GetGuildMember(parentId, id));
                        }
                    }
                    break;
                    //TODO: User Flyout
            }
            return flyout;
        }

        public static Flyout MakeUserDetailsFlyout(GuildMember member)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = member,
                DMPane = false
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
            return flyout;
        }

        public static Flyout MakeUserDetailsFlyout(User user)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = new GuildMember() { User = user },
                DMPane = false
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
            return flyout;
        }

        #region FlyoutCommands
        #region Profile
        public static void OpenProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile(((sender as MenuFlyoutItem).Tag as Nullable<SharedModels.User>).Value);
        }
        #endregion

        #region SubPages

        #region Guild
        public static void EditServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuildEdit((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void DeleteServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToDeleteServer((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void LeaveServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToLeaveServer((sender as MenuFlyoutItem).Tag.ToString());
        }
        #endregion

        #region Channel
        public static void EditChannel(object sender, RoutedEventArgs e)
        {
            App.NavigateToChannelEdit((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void DeleteChannel (object sender, RoutedEventArgs e)
        {
            App.NavigateToDeleteChannel((sender as MenuFlyoutItem).Tag.ToString());
        }
        #endregion

        #endregion

        #region Navigation

        public static void MessageUser(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel((sender as MenuFlyoutItem).Tag.ToString(), null, false, false, true);
        }

        public static async void KickMember(object sender, RoutedEventArgs e)
        {
            await RESTCalls.RemoveGuildMember(App.CurrentGuildId, (sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void BanMember(object sender, RoutedEventArgs e)
        {
            App.NavigateToCreateBan((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void ChangeNickname(object sender, RoutedEventArgs e)
        {
            App.NavigateToNicknameEdit((sender as MenuFlyoutItem).Tag.ToString());
        }
        #endregion

        #region RPC
        public static void MarkGuildasRead(object sender, RoutedEventArgs e)
        {
            App.MarkGuildAsRead((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void MuteServer(object sender, RoutedEventArgs e)
        {
            App.MuteGuild((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void MarkChannelasRead(object sender, RoutedEventArgs e)
        {
            App.MarkChannelAsRead((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void MuteChannel(object sender, RoutedEventArgs e)
        {
            App.MuteChannel((sender as MenuFlyoutItem).Tag.ToString());
        }
        #endregion

        #region API

        #region Relations
        public static void AddFriend(object sender, RoutedEventArgs e)
        {
            App.AddFriend((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void RemoveFriend(object sender, RoutedEventArgs e)
        {
            App.RemoveFriend((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void BlockUser(object sender, RoutedEventArgs e)
        {
            App.BlockUser((sender as MenuFlyoutItem).Tag.ToString());
        }
        #endregion

        public static async void InviteToServer(object sender, RoutedEventArgs e)
        {
            var invite = await RESTCalls.CreateInvite(((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item1, new CreateInvite() { MaxUses = 1, Temporary = false, Unique = true });
            App.NavigateToDMChannel(((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2, "https://discord.gg/" + invite.String, true, false, true);
        }

        public static async void AddRole(object sender, RoutedEventArgs e)
        {
            var modify = new API.Guild.Models.ModifyGuildMember(LocalState.Guilds[App.CurrentGuildId].members[((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2]);
            modify.ToggleRole((((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item1));
            await RESTCalls.ModifyGuildMember(App.CurrentGuildId, ((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2, modify);
        }

        public static async void RemoveGroupUser(object sender, RoutedEventArgs e)
        {
            var senderTag = ((sender as MenuFlyoutItem).Tag as Tuple<string, string>);
            await RESTCalls.RemoveGroupUser(senderTag.Item1, senderTag.Item2);
        }
        #endregion

        #endregion
    }
}
