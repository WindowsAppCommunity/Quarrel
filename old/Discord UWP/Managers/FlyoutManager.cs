using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Quarrel.Controls;
using Quarrel.Flyouts;
using Quarrel.LocalModels;
using DiscordAPI.SharedModels;

namespace Quarrel.Managers
{
    public class FlyoutManager
    {
        /// <summary>
        /// Types of flyouts
        /// </summary>
        public enum Type { Guild, GuildMember, GroupMember, Category, TextChn, DMChn, GroupChn, VoiceMember, SavePicture }

        /// <summary>
        /// Show Flyout for User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>MenuFlyout</returns>
        public static MenuFlyout ShowMenu(User user)
        {
            MenuFlyout flyout = new MenuFlyout();

            // Create flyout
            flyout = FlyoutCreator.MakeGuildMemberMenu(new GuildMember()
            {
                User = user
            });

            // Light dismiss for cinematic
            if (App.CinematicMode)
                flyout.LightDismissOverlayMode = LightDismissOverlayMode.On;

            // Style items
            foreach (var item in flyout.Items)
            {
                if (item.GetType() == typeof(MenuFlyoutItem))
                    item.Style = (Style)Application.Current.Resources["MenuFlyoutItemStyle1"];
            }
            
            // Return flyout
            return flyout;
        }

        public static async Task<MenuFlyout> ShowMenu(Type type, string id, string parentId)
        {
            MenuFlyout flyout = new MenuFlyout();

            // Handle flyout type
            switch (type)
            {
                // Guild
                case Type.Guild:
                    if (id != "@me")
                    {
                        flyout = FlyoutCreator.MakeGuildMenu(LocalState.Guilds[id]);
                    }
                    break;
                // DM Channel
                case Type.DMChn:
                    flyout = FlyoutCreator.MakeDMChannelMenu(LocalState.DMs[id]);
                    break;
                // Group Channel
                case Type.GroupChn:
                    flyout = FlyoutCreator.MakeGroupChannelMenu(LocalState.DMs[id]);
                    break;
                // Category
                case Type.Category:
                    if (parentId != null)
                    {
                        flyout = FlyoutCreator.MakeCategoryMenu(LocalState.Guilds[parentId].channels[id].raw, parentId);
                    }
                    break;
                // Text Channel
                case Type.TextChn:
                    if (parentId != null)
                    {
                        flyout = FlyoutCreator.MakeTextChnMenu(LocalState.Guilds[parentId].channels[id]);
                    }
                    break;
                // Guild Member
                case Type.GuildMember:
                    if (parentId != null)
                    {
                        if (LocalState.Guilds[parentId].members.ContainsKey(id))
                        {
                            flyout = FlyoutCreator.MakeGuildMemberMenu(LocalState.Guilds[parentId].members[id]);
                        }
                    }
                    break;
                // Voice Member
                case Type.VoiceMember:
                    if (parentId != null)
                    {
                        if (LocalState.Guilds[parentId].members.ContainsKey(id))
                        {
                            flyout = FlyoutCreator.MakeVoiceMemberMenu(LocalState.Guilds[parentId].members[id]);
                        } else
                        {
                            flyout = FlyoutCreator.MakeGuildMemberMenu(await RESTCalls.GetGuildMember(parentId, id));
                        }
                    }
                    break;
            }

            // Light dismiss for cinematic
            if (App.CinematicMode)
                flyout.LightDismissOverlayMode = LightDismissOverlayMode.On;

            // Style items
            foreach (var item in flyout.Items)
            {
                if(item.GetType() == typeof(MenuFlyoutItem))
                    item.Style = (Style)Application.Current.Resources["MenuFlyoutItemStyle1"];
            }

            // Return flyout
            return flyout;
        }

        /// <summary>
        /// Show Save Picture flyout
        /// </summary>
        /// <param name="url">Picture url</param>
        /// <returns>Save picture flyout</returns>
        public static MenuFlyout ShowMenu(string url)
        {
            return FlyoutCreator.MakeSavePictureFlyout(url);
        }

        public static Flyout MakeUserDetailsFlyout(GuildMember member, bool webhook)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = member,
                DMPane = false,
                Webhook = webhook
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyleUserControl"];
          
            return flyout;
        }

        public static Flyout MakeUserDetailsFlyout(User user, bool webhook)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new UserDetailsControl()
            {
                DisplayedMember = new GuildMember() { User = user },
                DMPane = false,
                Webhook = webhook
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyleUserControl"];
            return flyout;
        }

        public static Flyout MakeGameFlyout(string id)
        {
            Flyout flyout = new Flyout();
            flyout.Content = new GameDetailsControl()
            {
                GameId = id
            };
            flyout.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyleUserControl"];
            return flyout;
        }

        #region FlyoutCommands

        #region Profile
        public static void OpenProfile(object sender, RoutedEventArgs e)
        {
            App.NavigateToProfile(((sender as MenuFlyoutItem).Tag as DiscordAPI.SharedModels.User));
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

        public static async void MessageUser(object sender, RoutedEventArgs e)
        {
            string channelid = null;
            foreach (var dm in LocalState.DMs)
                if (dm.Value.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == (sender as MenuFlyoutItem).Tag.ToString())
                    channelid = dm.Value.Id;
            if(channelid == null)
                channelid = (await RESTCalls.CreateDM(new DiscordAPI.API.User.Models.CreateDM() { Recipients = new List<string>() { (sender as MenuFlyoutItem).Tag.ToString() }.AsEnumerable() })).Id;
            if (string.IsNullOrEmpty(channelid)) return;
            App.SelectGuildChannel("@me", channelid);
        }

        public static async void KickMember(object sender, RoutedEventArgs e)
        {
            try
            {
                await RESTCalls.RemoveGuildMember(App.CurrentGuildId, (sender as MenuFlyoutItem).Tag.ToString());
            }
            catch { }
        }

        public static void BanMember(object sender, RoutedEventArgs e)
        {
            App.NavigateToCreateBan((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void ChangeNickname(object sender, RoutedEventArgs e)
        {
            App.NavigateToNicknameEdit((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static async void OpenURL(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as MenuFlyoutItem).Tag.ToString()));
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

            var item = ((sender as MenuFlyoutItem));
        }

        public static void MarkChannelasRead(object sender, RoutedEventArgs e)
        {
            App.MarkChannelAsRead((sender as MenuFlyoutItem).Tag.ToString());
        }

        public static void MarkCategoryAsRead(object sender, RoutedEventArgs e)
        {
            var tuple = ((sender as MenuFlyoutItem).Tag as Tuple<string, string>);
            App.MarkCategoryAsRead(tuple.Item1, tuple.Item2);
        }

        public static void MuteChannel(object sender, RoutedEventArgs e)
        {
            App.MuteChannel((sender as MenuFlyoutItem).Tag.ToString());
            var item = ((sender as MenuFlyoutItem));
        }
        #endregion

        #region Messages

        public static void Reply(object sender, RoutedEventArgs e)
        {
            App.MentionUser(((sender as MenuFlyoutItem).Tag as Message).User.Username, ((sender as MenuFlyoutItem).Tag as Message).User.Discriminator);
        }

        private async void Pin(object sender, RoutedEventArgs e)
        {
            if (((sender as MenuFlyoutItem).Tag as Message).Pinned)
            {
                await RESTCalls.UnpinMessage(((sender as MenuFlyoutItem).Tag as Message).ChannelId, ((sender as MenuFlyoutItem).Tag as Message).Id);
            }
            else
            {
                await RESTCalls.PinMessage(((sender as MenuFlyoutItem).Tag as Message).ChannelId, ((sender as MenuFlyoutItem).Tag as Message).Id);
            }
        }

        //private void AddReaction(object sender, RoutedEventArgs e)
        //{
        //    Flyout PickReaction;
        //    PickReaction = new Flyout();
        //    EmojiControl emojiPicker = new EmojiControl();
        //    emojiPicker.PickedEmoji += ReactionSelected;
        //    PickReaction.FlyoutPresenterStyle = (Style)App.Current.Resources["FlyoutPresenterStyle1"];
        //    PickReaction.Content = emojiPicker;
        //    PickReaction.ShowAt(moreButton);
        //}

        //private void Edit(object sender, RoutedEventArgs e)
        //{

        //}

        private void Delete(object sender, RoutedEventArgs e)
        {
            App.DeleteMessage(((sender as MenuFlyoutItem).Tag as Message).ChannelId, ((sender as MenuFlyoutItem).Tag as Message).Id);
        }

        private void CopyId(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(((sender as MenuFlyoutItem).Tag as Message).Id);
            Clipboard.SetContent(dataPackage);
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
            var modify = new DiscordAPI.API.Guild.Models.ModifyGuildMember(LocalState.Guilds[App.CurrentGuildId].members[((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2]);
            modify.ToggleRole((((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item1));
            await RESTCalls.ModifyGuildMember(App.CurrentGuildId, ((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2, modify);
        }

        public static void LeaveUnownedChannel(object sender, RoutedEventArgs e)
        {
            App.NavigateToDeleteChannel(((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item1);
            //var senderTag = ((sender as MenuFlyoutItem).Tag as Tuple<string, string>);
            //await RESTCalls.DeleteChannel(senderTag.Item1);
        }

        public static void RemoveGroupUser(object sender, RoutedEventArgs e)
        {
            App.NavigateToRemoveGroupUser(((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item1, ((sender as MenuFlyoutItem).Tag as Tuple<string, string>).Item2);
            //var senderTag = ((sender as MenuFlyoutItem).Tag as Tuple<string, string>);
            //await RESTCalls.RemoveGroupUser(senderTag.Item1, senderTag.Item2);
        }
        #endregion

        #region Other

        public static async void SavePicture(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            savePicker.FileTypeChoices.Add("Photo", new List<string>()
            { "." + (sender as MenuFlyoutItem).Tag.ToString().Split('.').Last()});

            savePicker.SuggestedFileName = "Avatar";
            var file = await savePicker.PickSaveFileAsync();

            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(new Uri((sender as MenuFlyoutItem).Tag.ToString()), file);
            await download.StartAsync();
        }

        #endregion

        #endregion
    }
}
