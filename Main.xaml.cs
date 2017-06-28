/* Style Guide!!! (We need this)
 * Comments:
 * //Is used to comment-out any code while \**\ is used for notes
 * Old functions in a file (commented or active) should be put in a Region called OldCode and placed at the bottom of the file
 * If the commented code is midfunction just put new in Region OldCode around it
 * 
 * Object naming:
 * Properties and class instance object should be capitalized
 * Function specific objects should be lowercased with caps inplace of space 
 * Property objects should be named with an underscore infront then lowercase
 * 
 * Properties:
 * if a Property {get;} only returns the object call the Property (this is for Get References)
 * 
 * Random:
 * App.CurrentId should be used instead of ServerList.SelectedItem in everycase where it is not necessary
 * SharedModels and CacheModels should be included in all files that use them
 * CacheModels overrule SharedModels
 * Use lambdas meaningly, nothing is dumber than excessive lambda use
*/
using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Discord_UWP.Authentication;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static Discord_UWP.Common;
using Discord_UWP.CacheModels;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp;

#region CacheModels Overrule
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;
using Message = Discord_UWP.CacheModels.Message;
using User = Discord_UWP.CacheModels.User;
using Guild = Discord_UWP.CacheModels.Guild;
#endregion

/* The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238 */

namespace Discord_UWP
{
   /*<summary>
     An empty page that can be used on its own or navigated to within a Frame.
     </summary>*/
    
    public sealed partial class Main : Page
    {
        public async void Login(string args = null)
        {
            LoadingSplash.Show(false);
            UpdateUIfromSettings();
            try
            {
                LoadingSplash.Message = EntryMessages.GetMessage().ToUpper();
                LoadingSplash.Status = "LOGGING IN...";
                await Session.AutoLogin();
                Session.Online = true;
                EstablishGateway();
                LoadingSplash.Show(false);
                LoadingSplash.Status = "LOADING...";

                LoadCache();
                LoadMessages();
                LoadMutedChannels();

                LoadUser();
                LoadGuilds();

                var licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                {
                    ShowAds = false;
                }

            }
            catch
            {
                LoadingSplash.Hide(false);
                ShowAds = false;
                FeedbackButton.Visibility = Visibility.Collapsed;
                IAPSButton.Visibility = Visibility.Collapsed;
                MessageDialog msg = new MessageDialog("You're offline, loading only cached data");
                Session.Online = false;
                await msg.ShowAsync();
            }
            if(args != null)
            {
                bool guildId = true;
                foreach (char c in args)
                {
                    if (c == ':')
                    {
                        guildId = false;
                    }
                    else if (guildId)
                    {
                        SelectGuildId += c;
                    }
                    else
                    {
                        SelectChannelId += c;
                    }
                }
                SelectChannel = true;
            }
            if (Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay || Servers.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                Servers.IsPaneOpen = true;
                MessageArea.Opacity = 0.5;
            }
        }
        public Main()
        {
            this.InitializeComponent();


            #region OldCode
            #region TypingCheckTimer
            //Timer timer = new Timer( async (object state) =>
            //{
            //    if (Session.Typers.Last().Timestamp < (DateTime.Now.Ticks * 10000) - 2)
            //    {
            //        Session.Typers.RemoveAt(Session.Typers.Count);
            //    }

            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        TypingIndicator.Text = "";
            //        if (Session.Typers.Count > 0)
            //        {
            //            foreach (SharedModels.TypingStart typing in Session.Typers)
            //            {
            //                if (TextChannels.SelectedItem != null && typing.channelId == ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id)
            //                {
            //                    TypingIndicator.Text += " " + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[typing.userId] + " ";
            //                }
            //            }
            //        }
            //    });

            //}, null, (int)TimeSpan.TicksPerSecond, Timeout.Infinite);
            #endregion
            #endregion

            Login();     
        }

        private void UpdateUIfromSettings()
        {
            if (Storage.Settings.AppBarAtBottom)
                VisualStateManager.GoToState(this, "AppBarAlignment_Bottom", false);
            else
                VisualStateManager.GoToState(this, "AppBarAlignment_Top", false);

            ResponsiveUI_M_Trigger.MinWindowWidth = Storage.Settings.RespUiM;
            ResponsiveUI_L_Trigger.MinWindowWidth = Storage.Settings.RespUiL;
            ResponsiveUI_XL_Trigger.MinWindowWidth = Storage.Settings.RespUiXl;
        }

        public Main(string args)
        {
            InitializeComponent();

            #region OldCode
            #region TypingCheckTimer
            //Timer timer = new Timer( async (object state) =>
            //{
            //    if (Session.Typers.Last().Timestamp < (DateTime.Now.Ticks * 10000) - 2)
            //    {
            //        Session.Typers.RemoveAt(Session.Typers.Count);
            //    }

            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        TypingIndicator.Text = "";
            //        if (Session.Typers.Count > 0)
            //        {
            //            foreach (SharedModels.TypingStart typing in Session.Typers)
            //            {
            //                if (TextChannels.SelectedItem != null && typing.channelId == ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id)
            //                {
            //                    TypingIndicator.Text += " " + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members
            //                    [typing.userId] + " ";
            //                }
            //            }
            //        }
            //    });

            //}, null, (int)TimeSpan.TicksPerSecond, Timeout.Infinite);
            #endregion
            #endregion

            Login(args);
        }

        async void EstablishGateway()
        {
            Session.Gateway.Ready += OnReady;
            Session.Gateway.MessageCreated += MessageCreated;
            Session.Gateway.MessageDeleted += MessageDeleted;
            Session.Gateway.MessageUpdated += MessageUpdated;

            Session.Gateway.GuildCreated += GuildCreated;
            Session.Gateway.GuildDeleted += GuildDeleted;
            Session.Gateway.GuildUpdated += GuildUpdated;

            Session.Gateway.GuildChannelCreated += GuildChannelCreated;
            Session.Gateway.GuildChannelDeleted += GuildChannelDeleted;
            Session.Gateway.GuildChannelUpdated += GuildChannelUpdated;

            Session.Gateway.GuildMemberAdded += GuildMemberAdded;
            Session.Gateway.GuildMemberRemoved += GuildMemberRemoved;
            Session.Gateway.GuildMemberUpdated += GuildMemberUpdated;

            Session.Gateway.DirectMessageChannelCreated += DirectMessageChannelCreated;
            Session.Gateway.DirectMessageChannelDeleted += DirectMessageChannelDeleted;

            Session.Gateway.PresenceUpdated += PresenceUpdated;
            Session.Gateway.TypingStarted += TypingStarted;
            try
            {
                await Session.Gateway.ConnectAsync();
                Session.SlowSpeeds = false;
                RefreshButton.Visibility = Visibility.Collapsed;
            } catch
            {
                Session.SlowSpeeds = true;
                RefreshButton.Visibility = Visibility.Visible;
            }
        }

        #region LoadUser
        private void LoadUser()
        {
            if (Storage.Cache.CurrentUser != null)
            {
                Username.Text = Storage.Cache.CurrentUser.Raw.Username;
                Discriminator.Text = "#" + Storage.Cache.CurrentUser.Raw.Discriminator;
                LargeUsername.Text = Username.Text;
                LargeDiscriminator.Text = Discriminator.Text;
            }

            if (Session.Online)
            {
                DownloadUser();
            } else
            {
                /*disable online functions*/
            }
        }
        private async void DownloadUser()
        {
            Storage.Cache.CurrentUser = new User(await Session.GetCurrentUser());

            Username.Text = Storage.Cache.CurrentUser.Raw.Username;
            Discriminator.Text = "#" + Storage.Cache.CurrentUser.Raw.Discriminator;
            LargeUsername.Text = Username.Text;
            LargeDiscriminator.Text = Discriminator.Text;

            ImageBrush image = new ImageBrush() {ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + Storage.Cache.CurrentUser.Raw.Id + "/" + Storage.Cache.CurrentUser.Raw.Avatar + ".jpg"))};
            Avatar.Fill = image;
            LargeAvatar.Fill = image;
            Storage.SaveCache();
        }
        #endregion

        #region LoadGuilds
        private void LoadGuilds()
        {
            ServerList.Items.Clear();
            ServerList.Items.Add(MakeDmIcon());
            ServerList.SelectedIndex = 0;

            List<UIElement> TempGuildList = new List<UIElement>();
            while (TempGuildList.Count < 100)
            {
                TempGuildList.Add(new Grid());
            }

            foreach (KeyValuePair<string, Guild> guild in Storage.Cache.Guilds)
            {
                TempGuildList.Add(GuildRender(guild.Value));
                //TempGuildList.RemoveAt(Storage.Cache.guildOrder[guild.Key]);
                //TempGuildList.Insert(Storage.Cache.guildOrder[guild.Key], GuildRender(guild.Value));
            }

            foreach (UIElement item in TempGuildList)
            {
                if (item is ListViewItem)
                {
                    ServerList.Items.Add(item);
                }
            }
            Storage.SaveCache();
            if (SelectChannel)
            {
                foreach (ListViewItem guild in ServerList.Items)
                {
                    if (guild.Tag.ToString() == SelectGuildId)
                    {
                        ServerList.SelectedItem = guild;
                    }
                }
            }
        }
        private UIElement MakeDmIcon()
        {
            ListViewItem item = new ListViewItem(){ Height = 64,
                Width =64,
                MinWidth =64,
                Tag = "DMs",
                HorizontalContentAlignment =HorizontalAlignment.Center,
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Content = "", FontSize = 22 };
            ToolTipService.SetToolTip(item, "Direct messages");
            return item;
        }
        #endregion

        #region LoadGuild
        private void CatchServerSelection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null) /*Called upon clearing*/
            {
                _onlyAllowOpeningPane = true;
                ToggleServerListFull(null, null);

                ServerName.Text = ToolTipService.GetToolTip((sender as ListView).SelectedItem as DependencyObject).ToString();
                TextChannels.Items.Clear();
                SendMessage.Visibility = Visibility.Collapsed;
                if (((sender as ListView).SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                {
                    Channels.Visibility = Visibility.Collapsed;
                    DMs.Visibility = Visibility.Visible;
                    if (Session.Online)
                    {
                        ChannelsLoading.IsActive = true;
                        DownloadDMs();
                    } else
                    {
                        LoadDMs();
                    }
                }
                else
                {
                    Channels.Visibility = Visibility.Visible;
                    DMs.Visibility = Visibility.Collapsed;
                    if (Session.Online)
                    {
                        ChannelsLoading.IsActive = true;
                        DownloadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    } else
                    {
                        LoadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    }
                    if (SelectChannel)
                    {
                        foreach (ListViewItem channel in TextChannels.Items)
                        {
                            if ((channel.Tag as GuildChannel).Raw.Id == SelectChannelId)
                            {
                                TextChannels.SelectedItem = channel;
                            }
                        }
                        SelectChannel = false;
                    }
                }
            }
        }

        #region Members
        public void LoadMembers(string id)
        {
            int totalrolecounter = 0;
            if (Storage.Cache.Guilds[id].RawGuild.Roles != null)
            {
                foreach (Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
                {
                    int rolecounter = 0;
                    foreach (Member m in Storage.Cache.Guilds[id].Members.Values)
                        if (m.Raw.Roles.FirstOrDefault() == role.Id) rolecounter++;
                    var roleAlt = role;
                    totalrolecounter += rolecounter;
                    roleAlt.MemberCount = rolecounter;
                    if (Storage.Cache.Guilds[id].Roles.ContainsKey(role.Id))
                    {
                        Storage.Cache.Guilds[id].Roles[role.Id] = roleAlt;
                    }
                    else
                    {
                        Storage.Cache.Guilds[id].Roles.Add(role.Id, roleAlt);
                    }
                }
                int everyonecounter = Storage.Cache.Guilds[id].Members.Count() - totalrolecounter;
                var memberscvs = Storage.Cache.Guilds[id].Members;
                foreach (Member m in memberscvs.Values)
                {
                    m.MemberDisplayedRole = GetRole(m.Raw.Roles.FirstOrDefault(), id, everyonecounter);
                    if (Session.PrecenseDict.ContainsKey(m.Raw.User.Id))
                    {
                        m.status = Session.PrecenseDict[m.Raw.User.Id];
                    } else
                    {
                        m.status = new Presence() { Status = "offline", Game = null};
                    }
                }
                if (Storage.Settings.ShowOfflineMembers)
                    MembersCVS.Source = memberscvs.GroupBy(m => m.Value.MemberDisplayedRole).OrderBy(m => m.Key.Position).ToList();
                else
                    MembersCVS.Source = memberscvs.SkipWhile(m => m.Value.status.Status == "offline").GroupBy(m => m.Value.MemberDisplayedRole).OrderBy(m => m.Key.Position).ToList();
                TempRoleCache.Clear();
            }
        }

        private List<DisplayedRole> TempRoleCache = new List<DisplayedRole>(); //This is as a temporary cache of roles to improve performance and not call Storage for every member
        private DisplayedRole GetRole(string roleid, string guildid, int everyonecounter)
        {
            var cachedRole = TempRoleCache.FirstOrDefault(x => x.Id == roleid);
            if (cachedRole != null) return cachedRole;
            else
            {
                if (roleid == null)
                {
                    var role = new DisplayedRole(roleid, 10000, "EVERYONE", everyonecounter, (SolidColorBrush)App.Current.Resources["Foreground"]);
                    TempRoleCache.Add(role);
                    return role;
                }
                else
                {
                    DisplayedRole role;
                    if (Storage.Cache.Guilds[guildid].Roles[roleid].Hoist)
                    {
                        var storageRole = Storage.Cache.Guilds[guildid].Roles[roleid];
                        role = new DisplayedRole(roleid, storageRole.Position, storageRole.Name.ToUpper(), storageRole.MemberCount, IntToColor(storageRole.Color));
                        TempRoleCache.Add(role);
                    } else
                    {
                        role = new DisplayedRole(roleid, 10000, "EVERYONE", everyonecounter, (SolidColorBrush)App.Current.Resources["Foreground"]);
                        TempRoleCache.Add(role);
                    }
                    return role;
                }

            }
        }

        #endregion

        #region Guild
        private void LoadGuild(string id)
        {
            if (Storage.Cache.Guilds[id] != null)
            {
                ChannelsLoading.IsActive = true;
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");

                Messages.Items.Clear();


                #region Permissions
                Permissions perms = new Permissions();
                if (Storage.Cache.Guilds[id].RawGuild.Roles != null)
                {
                    foreach (SharedModels.Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
                    {
                        if (Storage.Cache.Guilds[id].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                        {
                            if (Storage.Cache.Guilds[id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                            {
                                perms.GetPermissions(role, Storage.Cache.Guilds[id].RawGuild.Roles);
                            }
                            else
                            {
                                perms.GetPermissions(0);
                            }
                        }
                    }
                }
                #endregion

                if ((!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Storage.Cache.Guilds[id].RawGuild.OwnerId != Storage.Cache.CurrentUser.Raw.Id) || !Session.Online)
                {
                    AddChannelButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AddChannelButton.Visibility = Visibility.Visible;
                }

                #region Roles
                MembersCVS.Source = null;
                LoadMembers(id);
                #endregion

                if (Storage.Cache.Guilds[id].RawGuild.Presences != null)
                {
                    foreach (Presence presence in Storage.Cache.Guilds[id].RawGuild.Presences)
                    {
                        if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                        {
                            Session.PrecenseDict.Remove(presence.User.Id);
                        }
                        Session.PrecenseDict.Add(presence.User.Id, presence);
                    }
                }

                List<UIElement> channelListBuffer = new List<UIElement>();
                while (channelListBuffer.Count < 1000)
                {
                    channelListBuffer.Add(new Grid());
                }

                TextChannels.Items.Clear();
                foreach (KeyValuePair<string, GuildChannel> channel in Storage.Cache.Guilds[id].Channels)
                {
                    if (channel.Value.Raw.Type == 0)
                    {
                        channelListBuffer.RemoveAt(channel.Value.Raw.Position);
                        channelListBuffer.Insert(channel.Value.Raw.Position, ChannelRender(channel.Value, perms));
                    }
                }

                foreach (UIElement element in channelListBuffer)
                {
                    if (element is Grid)
                    {

                    }
                    else if (element is ListViewItem)
                    {
                        TextChannels.Items.Add(element);
                    }
                }
            } else
            {

            }

            ChannelsLoading.IsActive = false;
            MessageBox.IsEnabled = false;
            MessageBox.PlaceholderText = "Can't send messages while offline";
            if (TextChannels.Items.Count > 0)
            {
                NoGuildChannelsCached.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoGuildChannelsCached.Visibility = Visibility.Visible;
            }
            App.CurrentId = id;
        }
        private async void DownloadGuild(string id)
        {
            IEnumerable<GuildMember> members = await Session.GetGuildMembers(id);

            foreach (GuildMember member in members)
            {
                if (!Storage.Cache.Guilds[id].Members.ContainsKey(member.User.Id))
                {
                    Storage.Cache.Guilds[id].Members.Add(member.User.Id, new Member(member));
                }
                else
                {
                    Storage.Cache.Guilds[id].Members[member.User.Id] = new Member(member);
                }
            }

            Messages.Items.Clear();

            MembersCVS.Source = null;

            #region Permissions
            Permissions perms = new Permissions();
            foreach (Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
            {
                if (!Storage.Cache.Guilds[id].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                {
                    Storage.Cache.Guilds[id].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new Member(Session.GetGuildMember(id, Storage.Cache.CurrentUser.Raw.Id)));
                }
                if (Storage.Cache.Guilds[id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[id].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                {
                    perms.GetPermissions(role, Storage.Cache.Guilds[id].RawGuild.Roles);
                }
                else
                {
                    perms.GetPermissions(0);
                }
            }
            #endregion

            if (!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Storage.Cache.Guilds[id].RawGuild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
            {
                AddChannelButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddChannelButton.Visibility = Visibility.Visible;
            }

            #region Roles

            MembersCVS.Source = null;
            LoadMembers(id);
            App.CurrentId = id;
            #endregion

            if (Storage.Cache.Guilds[id].RawGuild.Presences != null)
            {
                foreach (Presence presence in Storage.Cache.Guilds[id].RawGuild.Presences)
                {
                    if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                    {
                        Session.PrecenseDict.Remove(presence.User.Id);
                    }
                    Session.PrecenseDict.Add(presence.User.Id, presence);
                }
            }

            #region Channels
            List<UIElement> channelListBuffer = new List<UIElement>();
            while (channelListBuffer.Count < 1000)
            {
                channelListBuffer.Add(new Grid());
            }

            TextChannels.Items.Clear();
            foreach (KeyValuePair<string, GuildChannel> channel in Storage.Cache.Guilds[id].Channels)
            {
                if (channel.Value.Raw.Type == 0)
                {
                    channelListBuffer.RemoveAt(channel.Value.Raw.Position);
                    channelListBuffer.Insert(channel.Value.Raw.Position, ChannelRender(channel.Value, perms));
                }
            }

            foreach (UIElement element in channelListBuffer)
            {
                if (element is Grid)
                {

                }
                else if (element is ListViewItem)
                {
                    TextChannels.Items.Add(element);
                }
            }
            #endregion

            ChannelsLoading.IsActive = false;
        }
        #endregion

        #region DMs
        private void LoadDMs()
        {
            DMsLoading.IsActive = true;
            MembersCVS.Source = null;
            PinnedMessageToggle.Visibility = Visibility.Collapsed;
            SendMessage.Visibility = Visibility.Collapsed;
            MuteToggle.Visibility = Visibility.Collapsed;
            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, DmCache> channel in Storage.Cache.DMs)
            {
                DirectMessageChannels.Items.Add(ChannelRender(channel.Value));
            }
            DMsLoading.IsActive = false;
            if (DirectMessageChannels.Items.Count > 0)
            {
                NoDMSCached.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoDMSCached.Visibility = Visibility.Visible;
            }
            App.CurrentId = null;
        }
        private void DownloadDMs()
        {
            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, DmCache> dm in Storage.Cache.DMs)
            {
                DirectMessageChannels.Items.Add(ChannelRender(dm.Value));
            }
            DMsLoading.IsActive = false;
            App.CurrentId = null;
        }
        #endregion
        #endregion

        #region LoadChannel
        private async void LoadChannelMessages(object sender, SelectionChangedEventArgs e)
        {
            if (TextChannels.SelectedItem != null) /*Called upon clear*/
            {
                if (Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay || Servers.DisplayMode == SplitViewDisplayMode.Overlay)
                    Servers.IsPaneOpen = false;
                MessagesLoading.Visibility = Visibility.Visible;
                SendMessage.Visibility = Visibility.Visible;
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id);
                MuteToggle.Visibility = Visibility.Visible;

                Messages.Items.Clear();

                Messages.Items.Add(new MessageControl()); /*Necessary for no good reason*/

                int adCheck = 5;

                foreach (KeyValuePair<string, Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Messages.Reverse())
                {
                    adCheck--;
                    Messages.Items.Add(NewMessageContainer(message.Value.Raw, null, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        Messages.Items.Add(NewMessageContainer(null, null, true, null));
                        adCheck = 5;
                        await Task.Delay(100);
                    }
                }

                Messages.Items.RemoveAt(0);

                PinnedMessages.Items.Clear();

                foreach (KeyValuePair<string, Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].PinnedMessages.Reverse())
                {
                    adCheck--;
                    PinnedMessages.Items.Add(NewMessageContainer(message.Value.Raw, false, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        PinnedMessages.Items.Add(NewMessageContainer(null, false, true, null));
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "#" + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Raw.Name;

                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Raw.Topic;
                }
                else
                {
                    ChannelTopic.Text = "";
                }
                if (Session.Online)
                {
                    await DownloadChannelMessages();
                } else
                {
                    MessagesLoading.Visibility = Visibility.Collapsed;
                    if (Messages.Items.Count > 0)
                    {
                        NoMessageChached.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        NoMessageChached.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        private async Task DownloadChannelMessages()
        {
            if (TextChannels.SelectedItem != null)
            {
                SendMessage.Visibility = Visibility.Visible;
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id);
                MuteToggle.Visibility = Visibility.Visible;
                Messages.Items.Clear();
                IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id);

                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Messages.Clear();

                while (messages == null)
                {
                    /*Messages may not be downloaded yet*/
                }

                foreach (SharedModels.Message message in messages)
                {
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Messages.Add(message.Id, new Message(message));
                }


                int adCheck = 5;

                Messages.Items.Add(new MessageControl()); //Necessary for no good reason

                //Normal messages
                foreach (KeyValuePair<string, Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as GuildChannel).Raw.Id].Messages.Reverse())
                {
                    adCheck--;
                    Messages.Items.Add(NewMessageContainer(message.Value.Raw, null, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        Messages.Items.Add(NewMessageContainer(null, null, true, null));
                        adCheck = 5;
                        await Task.Delay(100);
                    }
                }

                Messages.Items.RemoveAt(0);

                //Pinned messages
                PinnedMessages.Items.Clear();
                await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id);
                IEnumerable<SharedModels.Message> pinnedmessages = await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id);
                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].PinnedMessages.Clear();

                foreach (SharedModels.Message message in pinnedmessages)
                {
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].PinnedMessages.Add(message.Id, new Message(message));
                }

                adCheck = 5;

                foreach (KeyValuePair<string, Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].PinnedMessages.Reverse())
                {
                    adCheck--;
                    PinnedMessages.Items.Add(NewMessageContainer(message.Value.Raw, false, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        PinnedMessages.Items.Insert(1, NewMessageContainer(null, false, true, null));
                        adCheck = 5;
                        await Task.Delay(100);
                    }
                }

                ChannelName.Text = "#" + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].Raw.Name;

                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].Raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].Raw.Topic;
                }
                else
                {
                    ChannelTopic.Text = "";
                }

                if (TextChannels.SelectedItem != null && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id].Messages != null)
                {
                    if (Storage.RecentMessages.ContainsKey(((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id))
                    {
                        Storage.RecentMessages[((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id] = (Messages.Items.Last() as MessageContainer)?.Message?.Id;
                    }
                    else
                    {
                        var messageContainer = Messages.Items.Last() as MessageContainer;
                        if (messageContainer != null && (messageContainer.Message).HasValue)
                        {
                            Storage.RecentMessages.Add(((TextChannels.SelectedItem as ListViewItem)?.Tag as GuildChannel)?.Raw.Id, (Messages.Items.Last() as MessageContainer)?.Message?.Id);
                        }
                    }
                    Storage.SaveMessages();
                }
                Storage.SaveCache();

                MessagesLoading.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region LoadDMChannel
        private async void LoadDmChannelMessages(object sender, SelectionChangedEventArgs e)
        {
            if (DirectMessageChannels.SelectedItem != null)
            {
                if (Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay || Servers.DisplayMode == SplitViewDisplayMode.Overlay)
                    Servers.IsPaneOpen = false;
                MessagesLoading.Visibility = Visibility.Visible;
                SendMessage.Visibility = Visibility.Visible;
                SendBox.IsEnabled = false;
                MuteToggle.Visibility = Visibility.Collapsed;

                Messages.Items.Clear();
                int adCheck = 5;

                foreach (KeyValuePair<string, Message> message in Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id].Messages.Reverse())
                {
                    adCheck--;
                    Messages.Items.Add(NewMessageContainer(message.Value.Raw, null, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        Messages.Items.Add(NewMessageContainer(null, null, true, null));
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "@" + Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id].Raw.Users.FirstOrDefault().Username;
                ChannelTopic.Text = "";

                if (Session.Online)
                {
                    await DownloadDmChannelMessages();
                } else
                {
                    MessagesLoading.Visibility = Visibility.Collapsed;
                    if (Messages.Items.Count > 0)
                    {
                        NoMessageChached.Visibility = Visibility.Collapsed;
                    } else
                    {
                        NoMessageChached.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        private async Task DownloadDmChannelMessages()
        {
            SendMessage.Visibility = Visibility.Visible;
            MuteToggle.Visibility = Visibility.Collapsed;

            Messages.Items.Clear();
            int adCheck = 5;

            IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id);

            if (Storage.Cache.DMs != null)
            {
                Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id].Messages.Clear();
            }

            foreach (SharedModels.Message message in messages)
            {
                Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id].Messages.Add(message.Id, new Message(message));
            }

            Messages.Items.Add(new MessageControl()); //Necessary for no good reason

            foreach (KeyValuePair<string, Message> message in Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as DmCache).Raw.Id].Messages.Reverse())
            {
                adCheck--;
                Messages.Items.Add(NewMessageContainer(message.Value.Raw, null, false, null));
                if (adCheck == 0 && ShowAds)
                {
                    Messages.Items.Add(NewMessageContainer(null, null, true, null));
                    adCheck = 5;
                }
            }

            Messages.Items.RemoveAt(0);

            if (DirectMessageChannels.SelectedItem != null && Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache).Raw.Id].Messages != null)
            {
                if (Storage.RecentMessages.ContainsKey(((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id))
                {
                    Storage.RecentMessages[((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw
                        .Id] = (Messages.Items.Last() as SharedModels.Message?)?.Id;
                }
                else
                {
                    Storage.RecentMessages.Add(((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as DmCache)?.Raw.Id, (Messages.Items.Last() as SharedModels.Message?)?.Id);
                }
                Storage.SaveMessages();
            }

            MessagesLoading.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region General
        bool _onlyAllowOpeningPane = false;
        private void ToggleServerListFull(object sender, RoutedEventArgs e)
        {
            if (!Servers.IsPaneOpen && (Servers.DisplayMode == SplitViewDisplayMode.Overlay || Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay))
                DarkenMessageArea.Begin();
            if (Servers.DisplayMode == SplitViewDisplayMode.CompactInline)
            {
                if (Servers.IsPaneOpen && !_onlyAllowOpeningPane)
                {
                    Servers.IsPaneOpen = false;
                    MessageArea.Margin = new Thickness(64, MessageArea.Margin.Top, MessageArea.Margin.Right, MessageArea.Margin.Bottom);
                }
                else
                {
                    Servers.IsPaneOpen = true;
                    MessageArea.Margin = new Thickness(320, MessageArea.Margin.Top, MessageArea.Margin.Right, MessageArea.Margin.Bottom);
                }
            }
            else if(Servers.IsPaneOpen && !_onlyAllowOpeningPane )
                Servers.IsPaneOpen = false;
            else
                Servers.IsPaneOpen = true;

            _onlyAllowOpeningPane = false;
        }
        private async void Refresh(object sender, RoutedEventArgs e)
        {
            if ((ServerList.SelectedItem as ListViewItem)?.Tag.ToString() != null)
            {
                await DownloadDmChannelMessages();
            }
            else {
                await DownloadChannelMessages();
            }
        }
        private void TogglePeopleShow(object sender, RoutedEventArgs e)
        {
            if (!Members.IsPaneOpen && (Members.DisplayMode == SplitViewDisplayMode.Overlay || Members.DisplayMode == SplitViewDisplayMode.CompactOverlay))
                DarkenMessageArea.Begin();
            if (Members.DisplayMode == SplitViewDisplayMode.Inline)
            {
                if (Members.IsPaneOpen)
                {
                    Members.IsPaneOpen = false;
                    MessageArea.Margin = new Thickness(MessageArea.Margin.Left, MessageArea.Margin.Top, 0, MessageArea.Margin.Bottom);
                }
                else
                {
                    Members.IsPaneOpen = true;
                    MessageArea.Margin = new Thickness(MessageArea.Margin.Left, MessageArea.Margin.Top, 240, MessageArea.Margin.Bottom);
                }
            }
            else
            {
                Members.IsPaneOpen = !Members.IsPaneOpen;
            }
        }
        private void TogglePinnedShow(object sender, RoutedEventArgs e)
        {

           // PinnedMessagesPopup.IsPaneOpen = !PinnedMessagesPopup.IsPaneOpen;
        }
        private async void LoadMoreMessages(object sender, TappedRoutedEventArgs e)
        {
            if (Messages.Items.Count > 0)
            {
                IEnumerable<SharedModels.Message> newMessages = await Session.GetChannelMessagesBefore(App.CurrentId, (Messages.Items[0] as MessageContainer).Message.Value.Id);

                int adCheck = 5;

                foreach (SharedModels.Message msg in newMessages)
                {
                    adCheck--;
                    Messages.Items.Insert(0, msg);
                    if (adCheck == 0 && ShowAds)
                    {
                        StackPanel adstack = new StackPanel();
                        adstack.Orientation = Orientation.Horizontal;
                        TextBlock txt = new TextBlock();
                        txt.Text = "";
                        adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Center;
                        ad.Margin = new Thickness(0, 6, 0, 6);
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        ad.Background = (SolidColorBrush)App.Current.Resources["DarkBG"];
                        adstack.Children.Add(ad);
                        Messages.Items.Insert(1, adstack);
                        adCheck = 5;
                    }
                }
            }
        }
        private async void OpenFeedbackHub(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        private async void OpenTwitter(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://twitter.com/AvishaiDernis"));
        }
        private async void OpenDiscordWeb(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.gg/HWTEfjW"));
        }
        private void OpenWhatsNew(object sender, RoutedEventArgs e)
        {
            DarkenMessageArea.Begin();
            WhatsNew.IsPaneOpen = true;
        }
        private void OpenIaPs(object sender, RoutedEventArgs e)
        {
            DarkenMessageArea.Begin();
            IAPs.IsPaneOpen = true;
        }
        private async void MakePurchase(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            switch ((sender as Button).Tag.ToString())
            {
                case "RemoveAds":
                    if (!licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("RemoveAds");

                            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog("Bought");
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog("Add-On was not purchased.");
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog("An error occured, try again later");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Polite":
                    if (!licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("Polite Dontation");

                            if (licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog("Bought");
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog("Add-On was not purchased.");
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog("An error occured, try again later");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Significant":
                    if (!licenseInformation.ProductLicenses["SignificantDontation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("SignificantDontation");

                            if (licenseInformation.ProductLicenses["SignificantDontation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog("Bought");
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog("Add-On was not purchased.");
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["SignificantDontation"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog("An error occured, try again later");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "OMGTHX":
                    if (!licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("OMGTHXDonation");

                            if (licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog("Bought");
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog("Add-On was not purchased.");
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog("An error occured, try again later");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Ridiculous":
                    if (!licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("RidiculousDonation");

                            if (licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog("Bought");
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog("Add-On was not purchased.");
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog("An error occured, try again later");
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
            }
        }
        #endregion

        #region GuildSettings
        private async void OpenGuildSettings(object sender, RoutedEventArgs e)
        {
            SharedModels.Guild guild = await Session.GetGuild((sender as Button).Tag.ToString());
            ServerNameChange.Text = guild.Name;
            ServerNameChange.PlaceholderText = guild.Name;
            _settingsPaneId = (sender as Button).Tag.ToString();
            RoleList.Items.Clear();
            foreach (SharedModels.Role role in guild.Roles)
            {
                ListViewItem listviewitem = new ListViewItem();
                listviewitem.Content = role.Name;
                if (role.Color != 0)
                {
                    listviewitem.Foreground = IntToColor(role.Color);
                }
                listviewitem.Tag = role.Id;
                RoleList.Items.Add(listviewitem);
            }
            GuildSettings.IsPaneOpen = true;
        }
        private void SaveGuildSettings(object sender, RoutedEventArgs e)
        {
            Session.ModifyGuild(_settingsPaneId, ServerNameChange.Text); //TODO: Fix
            GuildSettings.IsPaneOpen = false;
        }
        private void CloseGuildSettings(object sender, RoutedEventArgs e)
        {
            GuildSettings.IsPaneOpen = false;
        }
        private void SwitchEditRoleView(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null) //Called upon cleared
            {
                RolePermissionsList.Visibility = Visibility.Visible;
                Permissions perms = new Permissions();
                bool manageable = false;
                foreach (SharedModels.Role role in Storage.Cache.Guilds[_settingsPaneId].RawGuild.Roles)
                {
                    if (role.Id == ((sender as ListView).SelectedItem as ListViewItem).Tag.ToString())
                    {
                        perms.GetPermissions(role, Storage.Cache.Guilds[_settingsPaneId].RawGuild.Roles);
                    }
                }

                RoleAllowAnyoneToMention.Visibility = Visibility.Collapsed;
                RoleHoist.Visibility = Visibility.Collapsed;

                RoleAddReactions.IsChecked = perms.ServerSidePerms.AddReactions;
                if (!manageable)
                {
                    RoleAddReactions.IsEnabled = false;
                }
                RoleAdmininstrator.IsChecked = perms.ServerSidePerms.Administrator;
                if (!manageable)
                {
                    RoleAdmininstrator.IsEnabled = false;
                }
                RoleAttachFiles.IsChecked = perms.ServerSidePerms.AttachFiles;
                if (!manageable)
                {
                    RoleAttachFiles.IsEnabled = false;
                }
                RoleBanMembers.IsChecked = perms.ServerSidePerms.BanMembers;
                if (!manageable)
                {
                    RoleBanMembers.IsEnabled = false;
                }
                RoleChangeNickname.IsChecked = perms.ServerSidePerms.ChangeNickname;
                if (!manageable)
                {
                    RoleChangeNickname.IsEnabled = false;
                }
                RoleConnect.IsChecked = perms.ServerSidePerms.Connect;
                if (!manageable)
                {
                    RoleConnect.IsEnabled = false;
                }
                RoleCreateInstantInvite.IsChecked = perms.ServerSidePerms.CreateInstantInvite;
                if (!manageable)
                {
                    RoleCreateInstantInvite.IsEnabled = false;
                }
                RoleDeafenMembers.IsChecked = perms.ServerSidePerms.DeafenMembers;
                if (!manageable)
                {
                    RoleDeafenMembers.IsEnabled = false;
                }
                RoleKickMembers.IsChecked = perms.ServerSidePerms.KickMembers;
                if (!manageable)
                {
                    RoleKickMembers.IsEnabled = false;
                }
                RoleManageChannels.IsChecked = perms.ServerSidePerms.ManageChannels;
                if (!manageable)
                {
                    RoleManageChannels.IsEnabled = false;
                }
                RoleManageEmojis.IsChecked = perms.ServerSidePerms.ManageEmojis;
                if (!manageable)
                {
                    RoleManageEmojis.IsEnabled = false;
                }
                RoleManageGuild.IsChecked = perms.ServerSidePerms.ManangeGuild;
                if (!manageable)
                {
                    RoleManageGuild.IsEnabled = false;
                }
                RoleManageNicknames.IsChecked = perms.ServerSidePerms.ManageNicknames;
                if (!manageable)
                {
                    RoleManageNicknames.IsEnabled = false;
                }
                RoleManageRoles.IsChecked = perms.ServerSidePerms.ManageRoles;
                if (!manageable)
                {
                    RoleManageRoles.IsEnabled = false;
                }
                RoleMentionEveryone.IsChecked = perms.ServerSidePerms.MentionEveryone;
                if (!manageable)
                {
                    RoleMentionEveryone.IsEnabled = false;
                }
                RoleMoveMembers.IsChecked = perms.ServerSidePerms.MoveMembers;
                if (!manageable)
                {
                    RoleMoveMembers.IsEnabled = false;
                }
                RoleMuteMembers.IsChecked = perms.ServerSidePerms.MuteMembers;
                if (!manageable)
                {
                    RoleMuteMembers.IsEnabled = false;
                }
                RoleReadMessageHistory.IsChecked = perms.ServerSidePerms.ReadMessageHistory;
                if (!manageable)
                {
                    RoleReadMessageHistory.IsEnabled = false;
                }
                RoleSpeak.IsChecked = perms.ServerSidePerms.Speak;
                if (!manageable)
                {
                    RoleSpeak.IsEnabled = false;
                }
                RoleUseExternalEmojis.IsChecked = perms.ServerSidePerms.UseExternalEmojis;
                if (!manageable)
                {
                    RoleUseExternalEmojis.IsEnabled = false;
                }
                RoleUseVoiceActivity.IsChecked = perms.ServerSidePerms.UseVad;
                if (!manageable)
                {
                    RoleUseVoiceActivity.IsEnabled = false;
                }
            }
        }
        #endregion
        
        #region ChannelSettings
        private void OpenChannelSettings(object sender, RoutedEventArgs e)
        {
            SharedModels.GuildChannel channel = Session.GetGuildChannel((sender as Button).Tag.ToString());
            ChannelNameChange.Text = channel.Name;
            ChannelNameChange.PlaceholderText = channel.Name;
            if (channel.Topic != null)
            {
                ChannelTopicChnage.Text = channel.Topic;
                ChannelTopicChnage.PlaceholderText = channel.Topic;
            }
            _settingsPaneId = (sender as Button).Tag.ToString();
            ChannelSettings.IsPaneOpen = true;
            DarkenMessageArea.Begin();
            if (!Session.Online)
            {
                SaveChannelSettingsButton.IsEnabled = false;
            }
        }
        private void SaveChannelSettings(object sender, RoutedEventArgs e)
        {
            Session.ModifyGuildChannel(_settingsPaneId, ChannelNameChange.Text, ChannelTopicChnage.Text);
            ChannelSettings.IsPaneOpen = false;
        }
        private void CloseChannelSettings(object sender, RoutedEventArgs e)
        {
            ChannelSettings.IsPaneOpen = false;
        }
        private void SaveCreateChannel(object sender, RoutedEventArgs e)
        {
            Session.CreateChannel((ServerList.SelectedItem as ListViewItem).Tag.ToString(), CreateChannelName.Text);
            CreateChannel.IsPaneOpen = false;
        }
        private void CloaseCreateChannel(object sender, RoutedEventArgs e)
        {
            CreateChannel.IsPaneOpen = false;
        }
        private async void CheckDeleteChannel(object sender, RoutedEventArgs e)
        {
            MessageDialog winnerAnounce = new MessageDialog("Are you sure? The channel and all it's messages cannot be recovered");
            winnerAnounce.Commands.Add(new UICommand(
        "Delete",
        new UICommandInvokedHandler(ConfirmDelete)));
            winnerAnounce.Commands.Add(new UICommand(
                "No",
                new UICommandInvokedHandler(CancelDelete)));
            await winnerAnounce.ShowAsync();
        }
        private void CancelDelete(IUICommand command)
        {

        }
        private void ConfirmDelete(IUICommand command)
        {
            Session.DeleteChannel(_settingsPaneId);
            ChannelSettings.IsPaneOpen = false;
        }
        #endregion

        #region UserSettings
        private void OpenUserSettings(object sender, RoutedEventArgs e)
        {
            #region OldCode
            //   LockChannelList.IsOn = Storage.settings.LockChannels;
            //   if (Storage.settings.LockChannels)
            //  {
            //      AutoHideChannels.IsEnabled = false;
            //   }
            //  AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
            //   AutoHidePeople.IsOn = Storage.settings.AutoHidePeople;
            #endregion
            HighlightEveryone.IsOn = Storage.Settings.HighlightEveryone;
            Toasts.IsOn = Storage.Settings.Toasts;

            RespUI_M.Value = Storage.Settings.RespUiM;
            RespUI_L.Value = Storage.Settings.RespUiL;
            RespUI_XL.Value = Storage.Settings.RespUiXl;
            AppBarAtBottom_checkbox.IsChecked = Storage.Settings.AppBarAtBottom;
            accent_combobox.SelectedItem = accent_combobox.Items.FirstOrDefault(x => (((ComboBoxItem)x).Tag as SolidColorBrush).Color.ToHex() == Storage.Settings.AccentBrush);

            if (Storage.Settings.Theme == Theme.Dark)
                radio_Dark.IsChecked = true;
            else if (Storage.Settings.Theme == Theme.Light)
                radio_Light.IsChecked = true;
            else if (Storage.Settings.Theme == Theme.Auto)
                radio_Auto.IsChecked = true;


            DarkenMessageArea.Begin();
            UserSettings.IsPaneOpen = !UserSettings.IsPaneOpen;
        }

        private void SaveUserSettings(object sender, RoutedEventArgs e)
        {
            #region OldCode
            //Storage.settings.AutoHideChannels = AutoHideChannels.IsOn;
            //Storage.settings.AutoHidePeople = AutoHidePeople.IsOn;
            #endregion
            Storage.Settings.HighlightEveryone = HighlightEveryone.IsOn;
            Storage.Settings.Toasts = Toasts.IsOn;

            Storage.Settings.RespUiM = RespUI_M.Value;
            Storage.Settings.RespUiL = RespUI_L.Value;
            Storage.Settings.RespUiXl = RespUI_XL.Value;
            Storage.Settings.AppBarAtBottom = (bool)AppBarAtBottom_checkbox.IsChecked;
            Storage.Settings.AccentBrush = ((SolidColorBrush)(accent_combobox.SelectedItem as ComboBoxItem)?.Tag)?.Color.ToHex();

            if ((bool)radio_Dark.IsChecked)
                Storage.Settings.Theme = Theme.Dark;
            else if ((bool)radio_Light.IsChecked)
                Storage.Settings.Theme = Theme.Light;
            else if ((bool)radio_Auto.IsChecked)
                Storage.Settings.Theme = Theme.Auto;

            Storage.SaveAppSettings();
            UserSettings.IsPaneOpen = false;
            UpdateUIfromSettings();
        }

        private void CloseUserSettings(object sender, RoutedEventArgs e)
        {
            UserSettings.IsPaneOpen = false;
        }
        private async void CheckLogout(object sender, RoutedEventArgs e)
        {
            MessageDialog winnerAnounce = new MessageDialog("Are you sure? logging back in can be a hassel");
            winnerAnounce.Commands.Add(new UICommand(
        "Logout",
        new UICommandInvokedHandler(ConfirmLogout)));
            winnerAnounce.Commands.Add(new UICommand(
                "No",
                new UICommandInvokedHandler(CancelLogout)));
            await winnerAnounce.ShowAsync();
        }
        private void CancelLogout(IUICommand command)
        {

        }
        private void ConfirmLogout(IUICommand command)
        {
            Storage.Clear();
            Session.Logout();
            if (App.IsConsole)
            {
                Frame.Navigate(typeof(LockScreen), null);
            } else
            {
                Frame.Content = new LockScreen();
            }
        }
        #endregion

        public static bool ShowAds = true;
        public bool SelectChannel = false;
        public string SelectChannelId = "";
        public string SelectGuildId = "";
        string _settingsPaneId;

        private void SP_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            LightenMessageArea.Begin();
        }

        bool _ignoreRespUiChanges = false;
        private void RespUI_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_ignoreRespUiChanges)
            {
                if (RespUI_L.Value < RespUI_M.Value) RespUI_L.Value = RespUI_M.Value;
                if (RespUI_XL.Value < RespUI_L.Value) RespUI_XL.Value = RespUI_L.Value;
            }
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            _ignoreRespUiChanges = true;
            RespUI_M.Value = 569;
            RespUI_L.Value = 768;
            RespUI_XL.Value = 1024;
            _ignoreRespUiChanges = false;
        }

        private void ServerList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                /*This fires when the selected item is DMs*/
                if(e.ClickedItem.ToString() == "" && ServerList.SelectedIndex == 0)
                {
                    ToggleServerListFull(null, null);
                }
                /*And this fires when it's a normal item*/
                else if (ServerList.SelectedItem == (e.ClickedItem as StackPanel)?.Parent)
                {
                    ToggleServerListFull(null, null);
                }
            }
            catch (Exception) { }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenChannelSettings(null,null);
        }

        private void Messages_RefreshRequested(object sender, EventArgs e)
        {
            LoadMoreMessages(null,null);
        }

        private async void ShowUserDetails(string id)
        {
           await new MessageDialog("Sorry, but this feature hasn't yet been added into Discord UWP, it will be available in the next update. The fact that we didn't add it means you got access to the rest of the app sooner ;)", "Coming soon™")
                .ShowAsync();
        }
        private async void MessageControl_OnLinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
        {
            if (e.Link.StartsWith("#"))
            {
                string val = e.Link.Remove(0, 1);
                foreach (ListViewItem item in TextChannels.Items)
                {
                    if ((item.Tag as GuildChannel).Raw.Id == val)
                    {
                        TextChannels.SelectedItem = item;
                    }
                }
            }
            else if (e.Link.StartsWith("@!"))
            {
                string val = e.Link.Remove(0, 2);
                ShowUserDetails(val);
            }
            else if (e.Link.StartsWith("@&"))
            {
                string val = e.Link.Remove(0, 2);
                ShowUserDetails(val);
            } else if (e.Link.StartsWith("@")){
                string val = e.Link.Remove(0, 1);
                ShowUserDetails(val);
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }

        private async void Upvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://aka.ms/Wp1zo6"));
        }

        private void Messages_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                args.ItemContainer.ContentTemplate = null;
                args.ItemContainer.DataContext = null;
            }

        }

        private void UpdateGame(Control sender, FocusDisengagedEventArgs args)
        {
            UserStatus_Checked(null, null);
        }

        private void UserStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (Playing != null) /*Called pre-full-initialization*/
            {
                if (UserStatusOnline.IsChecked == true)
                {
                    if (Playing.Text == "")
                    {
                        Session.Gateway.UpdateStatus("online", null, null);
                    }
                    else
                    {
                        Session.Gateway.UpdateStatus("online", null, new Game() { Name = Playing.Text == "" ? null : Playing.Text });
                    }
                    Playing.IsEnabled = true;
                }
                else if (UserStatusIdle.IsChecked == true)
                {
                    if (Playing.Text == "")
                    {
                        Session.Gateway.UpdateStatus("idle", 10000, null);
                    }
                    else
                    {
                        Session.Gateway.UpdateStatus("idle", 10000, new Game() { Name = Playing.Text == "" ? null : Playing.Text });
                    }
                    Playing.IsEnabled = true;
                }
                else if (UserStatusDND.IsChecked == true)
                {
                    if (Playing.Text == "")
                    {
                        Session.Gateway.UpdateStatus("dnd", null, null);
                    }
                    else
                    {
                        Session.Gateway.UpdateStatus("dnd", null, new Game() { Name = Playing.Text == "" ? null : Playing.Text });
                    }
                    Playing.IsEnabled = true;
                }
                else if (UserStatusInvisible.IsChecked == true)
                {
                    Session.Gateway.UpdateStatus("invisible", null, null);
                    Playing.IsEnabled = false;
                }
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            DarkenMessageArea.Begin();
         
            UserSettings.IsPaneOpen = true;
        }

        #region OldCode
        //private void LockChannelsToggled(object sender, RoutedEventArgs e)
        //{
        //    if ((sender as ToggleSwitch).IsOn)
        //    {
        //             AutoHideChannels.IsEnabled = false;
        //             AutoHideChannels.IsOn = false;
        //    }
        //    else
        //    {
        //             AutoHideChannels.IsEnabled = true;
        //             AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
        //    }
        //}

        //private void Scrolldown(object sender, SizeChangedEventArgs e)
        //{
        //    if (e.PreviousSize.Height < 10)
        //    {
        //          MessageScroller.ChangeView(0.0f, MessageScroller.ScrollableHeight, 1f);
        //    }
        //}
        #endregion

    //    private bool autoscrolldown = true;
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //if user is scrolled more than 20 pixels away from the bottom, disable automatic scrolling
          //  autoscrolldown = !((message_scroller.ScrollableHeight - message_scroller.VerticalOffset) > 20);
        }


        private void Messages_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // if(autoscrolldown)
            //    message_scroller.ChangeView(null, message_scroller.ScrollableHeight, null);
        }

        private void Message_scroller_OnViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            
        }

    }
}
