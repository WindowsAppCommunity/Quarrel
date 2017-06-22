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
using Discord_UWP.SharedModels;
using static Discord_UWP.Common;
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class Main : Page
    {
        public Main()
        {
            this.InitializeComponent();
            UpdateUIfromSettings();

            #region TypingCheckTimer
            /*Timer timer = new Timer( async (object state) =>
            {
                if (Session.typers.Last().Timestamp < (DateTime.Now.Ticks * 10000) - 2)
                {
                    Session.typers.RemoveAt(Session.typers.Count);
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    TypingIndicator.Text = "";
                    if (Session.typers.Count > 0)
                    {
                        foreach (SharedModels.TypingStart typing in Session.typers)
                        {
                            if (TextChannels.SelectedItem != null && typing.channelId == ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id)
                            {
                                TypingIndicator.Text += " " + Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].members[typing.userId] + " ";
                            }
                        }
                    }
                });

            }, null, (int)TimeSpan.TicksPerSecond, Timeout.Infinite);
            */
            #endregion

            LoadCache();
            LoadMessages();
            LoadMutedChannels();

            LoadUser();
            LoadGuilds();

            if (Session.Online)
            {
                EstablishGateway();
                var licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                {
                    ShowAds = false;
                }
            }
            else
            {
                ShowAds = false;
                FeedbackButton.Visibility = Visibility.Collapsed;
                IAPSButton.Visibility = Visibility.Collapsed;
            }
            if (Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay || Servers.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                Servers.IsPaneOpen = true;
                MessageArea.Opacity = 0.5;
            }
                
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

            #region TypingCheckTimer
            /*Timer timer = new Timer( async (object state) =>
            {
                if (Session.typers.Last().Timestamp < (DateTime.Now.Ticks * 10000) - 2)
                {
                    Session.typers.RemoveAt(Session.typers.Count);
                }

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    TypingIndicator.Text = "";
                    if (Session.typers.Count > 0)
                    {
                        foreach (SharedModels.TypingStart typing in Session.typers)
                        {
                            if (TextChannels.SelectedItem != null && typing.channelId == ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id)
                            {
                                TypingIndicator.Text += " " + Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].members[typing.userId] + " ";
                            }
                        }
                    }
                });

            }, null, (int)TimeSpan.TicksPerSecond, Timeout.Infinite);
            */
            #endregion

            LoadCache();
            LoadMessages();
            LoadMutedChannels();

            LoadUser();
            LoadGuilds();


            if (Session.Online)
            {
                EstablishGateway();
                var licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                {
                    ShowAds = false;
                }
            }
            else
            {
                ShowAds = false;
                FeedbackButton.Visibility = Visibility.Collapsed;
                IAPSButton.Visibility = Visibility.Collapsed;
            }
            

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
            }

            if (Session.Online)
            {
                DownloadUser();
            } else
            {
                //disable online functions
            }
        }
        private async void DownloadUser()
        {
            Storage.Cache.CurrentUser = new CacheModels.User(await Session.GetCurrentUser());

            Username.Text = Storage.Cache.CurrentUser.Raw.Username;
            Discriminator.Text = "#" + Storage.Cache.CurrentUser.Raw.Discriminator;

            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + Storage.Cache.CurrentUser.Raw.Id + "/" + Storage.Cache.CurrentUser.Raw.Avatar + ".jpg"));
            Avatar.Fill = image;
            Storage.SaveCache();
        }
        #endregion

        #region LoadGuilds
        private async void LoadGuilds()
        {
            ServerList.Items.Clear();
            ServerList.Items.Add(MakeDmIcon());
            ServerList.SelectedIndex = 0;
            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.Cache.Guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
            }
            if (Session.Online)
            {
                await DownloadGuilds();
            } else
            {
                //disable online functions
            }
        }
        private async Task DownloadGuilds()
        {
            IEnumerable<SharedModels.UserGuild> guilds = await Session.GetGuilds();

            foreach (SharedModels.UserGuild guild in guilds)
            {
                if (Storage.Cache.Guilds.ContainsKey(guild.Id))
                {
                    var channels = Storage.Cache.Guilds[guild.Id].Channels;
                    var members = Storage.Cache.Guilds[guild.Id].Members;
                    Storage.Cache.Guilds[guild.Id] = new CacheModels.Guild(guild);
                    Storage.Cache.Guilds[guild.Id].RawGuild = await Session.GetGuild(guild.Id);
                    Storage.Cache.Guilds[guild.Id].Channels = channels;
                    Storage.Cache.Guilds[guild.Id].Members = members;
                }
                else
                {
                    Storage.Cache.Guilds.Add(guild.Id, new CacheModels.Guild(guild));
                    Storage.Cache.Guilds[guild.Id].RawGuild = await Session.GetGuild(guild.Id);
                }
            }

            ServerList.Items.Clear();
            ServerList.Items.Add(MakeDmIcon());
            ServerList.SelectedIndex = 0;
            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.Cache.Guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
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
        private async void CatchServerSelection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null) //Called upon clearing
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
                        await DownloadDMs();
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
                        await DownloadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    } else
                    {
                        LoadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    }
                    if (SelectChannel)
                    {
                        foreach (ListViewItem channel in TextChannels.Items)
                        {
                            if ((channel.Tag as CacheModels.GuildChannel).Raw.Id == SelectChannelId)
                            {
                                TextChannels.SelectedItem = channel;
                            }
                        }
                        SelectChannel = false;
                    }
                }
            }
        }

        #region Guild
        private void LoadGuild(string id)
        {
            if (Storage.Cache.Guilds[id] != null)
            {
                ChannelsLoading.IsActive = true;
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");

                Messages.Items.Clear();

                if (MemberList != null)
                {
                    MemberList.Children.Clear();
                }

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
                List<ListView> memberListBuffer = new List<ListView>();
                while (memberListBuffer.Count < 1000)
                {
                    memberListBuffer.Add(new ListView());
                }

                if (Storage.Cache.Guilds[id].RawGuild.Roles != null)
                {
                    
                    Storage.Cache.Guilds[id].Roles.Clear();
                    foreach (SharedModels.Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
                    {
                        if (Storage.Cache.Guilds[id].Roles.ContainsKey(role.Id))
                        {
                            Storage.Cache.Guilds[id].Roles[role.Id] = role;
                        }
                        else
                        {
                            Storage.Cache.Guilds[id].Roles.Add(role.Id, role);
                        }

                        if (role.Hoist)
                        {
                            ListView listview = new ListView();
                            listview.Header = new TextBlock() { Text = role.Name.ToUpper(), TextWrapping = TextWrapping.Wrap, Opacity = 0.6, Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"], FontSize=13.333, Margin= new Thickness(12) };

                            listview.SelectionMode = ListViewSelectionMode.None;
                            listview.FontSize = 13.333;
                            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[id].Members)
                            {
                                if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                {
                                    ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                    listview.Items.Add(listviewitem);
                                }
                            }
                            memberListBuffer.Insert(1000 - role.Position * 3, listview);
                        }
                    }
                }

                foreach (ListView listview in memberListBuffer)
                {
                    if (listview.Items.Count != 0)
                    {
                        MemberList.Children.Add(listview);
                    }
                }

                ListView fulllistview = new ListView();
                fulllistview.Header = new TextBlock() { Text = "EVERYONE", TextWrapping = TextWrapping.Wrap, Opacity = 0.6, Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"], FontSize = 13.333, Margin = new Thickness(12) };

                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[id].Members)
                {
                    if (Storage.Cache.Guilds[id].Members.ContainsKey(member.Value.Raw.User.Id))
                    {
                        ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                        fulllistview.Items.Add(listviewitem);
                    }
                }
                MemberList.Children.Add(fulllistview);

                #endregion

                if (Storage.Cache.Guilds[id].RawGuild.Presences != null)
                {
                    foreach (SharedModels.Presence presence in Storage.Cache.Guilds[id].RawGuild.Presences)
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
                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[id].Channels)
                {
                    if (channel.Value.Raw.Type == "text")
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
        private async Task DownloadGuild(string id)
        {
            Storage.Cache.Guilds[id].RawGuild = await Session.GetGuild(id);

            IEnumerable<SharedModels.GuildMember> members = await Session.GetGuildMembers(id);

            foreach (SharedModels.GuildMember member in members)
            {
                if (!Storage.Cache.Guilds[id].Members.ContainsKey(member.User.Id))
                {
                    Storage.Cache.Guilds[id].Members.Add(member.User.Id, new CacheModels.Member(member));
                } else
                {
                    Storage.Cache.Guilds[id].Members[member.User.Id] = new CacheModels.Member(member);
                }
            }

            Messages.Items.Clear();

            if (MemberList != null)
            {
                MemberList.Children.Clear();
            }

            #region Permissions
            Permissions perms = new Permissions();
            foreach (SharedModels.Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
            {
                if (!Storage.Cache.Guilds[id].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                {
                    Storage.Cache.Guilds[id].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new CacheModels.Member(Session.GetGuildMember(id, Storage.Cache.CurrentUser.Raw.Id)));
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
            List<ListView> memberListBuffer = new List<ListView>();
            while (memberListBuffer.Count < 1000)
            {
                memberListBuffer.Add(new ListView());
            }

            if (Storage.Cache.Guilds[id].RawGuild.Roles != null)
            {
                Storage.Cache.Guilds[id].Roles.Clear();
                
                foreach (SharedModels.Role role in Storage.Cache.Guilds[id].RawGuild.Roles)
                {
                    if (Storage.Cache.Guilds[id].Roles.ContainsKey(role.Id))
                    {
                        Storage.Cache.Guilds[id].Roles[role.Id] = role;
                    }
                    else
                    {
                        Storage.Cache.Guilds[id].Roles.Add(role.Id, role);
                    }

                    if (role.Hoist)
                    {
                        ListView listview = new ListView();
                        listview.Header = new TextBlock() { Text = role.Name.ToUpper(), TextWrapping = TextWrapping.Wrap, Opacity = 0.6, Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"], FontSize = 13.333, Margin = new Thickness(12) };
                        listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                        listview.SelectionMode = ListViewSelectionMode.None;

                        foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[id].Members)
                        {
                            if (member.Value.Raw.Roles.Contains<string>(role.Id))
                            {
                                ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                listview.Items.Add(listviewitem);
                            }
                        }
                        memberListBuffer.Insert(1000 - role.Position * 3, listview);
                    }
                }
            }

            foreach (ListView listview in memberListBuffer)
            {
                if (listview.Items.Count != 0)
                {
                    MemberList.Children.Add(listview);
                }
            }

            ListView fulllistview = new ListView();
            fulllistview.Header = new TextBlock() { Text = "EVERYONE", TextWrapping = TextWrapping.Wrap, Opacity = 0.6, Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"], FontSize = 13.333, Margin = new Thickness(12) };
            fulllistview.SelectionMode = ListViewSelectionMode.None;

            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
            {

                if (Storage.Cache.Guilds[id].Members.ContainsKey(member.Value.Raw.User.Id))
                {
                    ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                    fulllistview.Items.Add(listviewitem);
                }
            }
            MemberList.Children.Add(fulllistview);
            App.CurrentId = id;
            #endregion

            if (Storage.Cache.Guilds[id].RawGuild.Presences != null)
            {
                foreach (SharedModels.Presence presence in Storage.Cache.Guilds[id].RawGuild.Presences)
                {
                    if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                    {
                        Session.PrecenseDict.Remove(presence.User.Id);
                    }
                    Session.PrecenseDict.Add(presence.User.Id, presence);
                }
            }

            #region Channels
            if ((ServerList.SelectedItem as ListViewItem).Tag != null)
            {
                IEnumerable<SharedModels.GuildChannel> channels = await Session.GetGuildData(id);
                Storage.Cache.Guilds[id].Channels.Clear();
                foreach (SharedModels.GuildChannel channel in channels)
                {
                    Storage.Cache.Guilds[id].Channels.Add(channel.Id, new CacheModels.GuildChannel(channel));
                }
            }


            List<UIElement> channelListBuffer = new List<UIElement>();
            while (channelListBuffer.Count < 1000)
            {
                channelListBuffer.Add(new Grid());
            }

            TextChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[id].Channels)
            {
                if (channel.Value.Raw.Type == "text")
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
            if (MemberList != null)
            {
                MemberList.Children.Clear();
            }
            PinnedMessageToggle.Visibility = Visibility.Collapsed;
            SendMessage.Visibility = Visibility.Collapsed;
            MuteToggle.Visibility = Visibility.Collapsed;
            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.DmCache> channel in Storage.Cache.DMs)
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
        private async Task DownloadDMs()
        {
            Storage.Cache.DMs.Clear();
            foreach (SharedModels.DirectMessageChannel dm in await Session.GetDMs())
            {
                if (!Storage.Cache.DMs.ContainsKey(dm.Id))
                {
                    Storage.Cache.DMs.Add(dm.Id, new CacheModels.DmCache(dm));
                }
            }

            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.DmCache> dm in Storage.Cache.DMs)
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
            if (TextChannels.SelectedItem != null) //Called upon clear
            {
                if (Servers.DisplayMode == SplitViewDisplayMode.CompactOverlay || Servers.DisplayMode == SplitViewDisplayMode.Overlay)
                    Servers.IsPaneOpen = false;
                MessagesLoading.Visibility = Visibility.Visible;
                SendMessage.Visibility = Visibility.Visible;
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);
                MuteToggle.Visibility = Visibility.Visible;

                Messages.Items.Clear();

                Messages.Items.Add(new MessageControl()); //Necessary for no good reason

                int adCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Reverse())
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

                PinnedMessages.Items.Clear();

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].PinnedMessages.Reverse())
                {
                    adCheck--;
                    PinnedMessages.Items.Add(NewMessageContainer(message.Value.Raw, false, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        PinnedMessages.Items.Add(NewMessageContainer(null, false, true, null));
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "#" + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Raw.Name;

                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Raw.Topic;
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
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);
                MuteToggle.Visibility = Visibility.Visible;
                Messages.Items.Clear();
                IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);

                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Clear();

                while (messages == null)
                {
                    //Messages may not be downloaded yet
                }

                foreach (SharedModels.Message message in messages)
                {
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Add(message.Id, new CacheModels.Message(message));
                }


                int adCheck = 5;

                Messages.Items.Add(new MessageControl()); //Necessary for no good reason

                //Normal messages
                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Reverse())
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

                //Pinned messages
                PinnedMessages.Items.Clear();
                await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id);
                IEnumerable<SharedModels.Message> pinnedmessages = await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id);
                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].PinnedMessages.Clear();

                foreach (SharedModels.Message message in pinnedmessages)
                {
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].PinnedMessages.Add(message.Id, new CacheModels.Message(message));
                }

                adCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].PinnedMessages.Reverse())
                {
                    adCheck--;
                    PinnedMessages.Items.Add(NewMessageContainer(message.Value.Raw, false, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        PinnedMessages.Items.Insert(1, NewMessageContainer(null, false, true, null));
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "#" + Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].Raw.Name;

                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].Raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].Raw.Topic;
                }
                else
                {
                    ChannelTopic.Text = "";
                }

                if (TextChannels.SelectedItem != null && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem)?.Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id].Messages != null)
                {
                    if (Storage.RecentMessages.ContainsKey(((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id))
                    {
                        Storage.RecentMessages[((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id] = (Messages.Items.Last() as MessageContainer)?.Message?.Id;
                    }
                    else
                    {
                        var messageContainer = Messages.Items.Last() as MessageContainer;
                        if (messageContainer != null && (messageContainer.Message).HasValue)
                        {
                            Storage.RecentMessages.Add(((TextChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.GuildChannel)?.Raw.Id, (Messages.Items.Last() as MessageContainer)?.Message?.Id);
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

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages.Reverse())
                {
                    adCheck--;
                    Messages.Items.Add(NewMessageContainer(message.Value.Raw, null, false, null));
                    if (adCheck == 0 && ShowAds)
                    {
                        Messages.Items.Add(NewMessageContainer(null, null, true, null));
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "@" + Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Raw.User.Username;
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

            IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id);

            if (Storage.Cache.DMs != null)
            {
                Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages.Clear();
            }

            foreach (SharedModels.Message message in messages)
            {
                Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages.Add(message.Id, new CacheModels.Message(message));
            }

            Messages.Items.Add(new MessageControl()); //Necessary for no good reason

            foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages.Reverse())
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

            if (DirectMessageChannels.SelectedItem != null && Storage.Cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.DmCache).Raw.Id].Messages != null)
            {
                if (Storage.RecentMessages.ContainsKey(((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.DmCache)?.Raw.Id))
                {
                    Storage.RecentMessages[((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.DmCache)?.Raw
                        .Id] = (Messages.Items.Last() as Message?)?.Id;
                }
                else
                {
                    Storage.RecentMessages.Add(((DirectMessageChannels.SelectedItem as ListViewItem)?.Tag as CacheModels.DmCache)?.Raw.Id, (Messages.Items.Last() as Message?)?.Id);
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
        private void Scrolldown(object sender, SizeChangedEventArgs e)
        {
          //  if (e.PreviousSize.Height < 10)
          //  {
          //      MessageScroller.ChangeView(0.0f, MessageScroller.ScrollableHeight, 1f);
          //  }
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
            IEnumerable<SharedModels.Message> newMessages = await Session.GetChannelMessagesBefore(App.CurrentId, (Messages.Items[2] as MessageContainer).Message.Value.Id);

            int adCheck = 5;

            foreach (SharedModels.Message msg in newMessages)
            {
                adCheck--;
                Messages.Items.Insert(1, msg);
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
            //     LockChannelList.IsOn = Storage.settings.LockChannels;
            //   if (Storage.settings.LockChannels)
            //  {
            //      AutoHideChannels.IsEnabled = false;
            //   }
            //  AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
            //   AutoHidePeople.IsOn = Storage.settings.AutoHidePeople;
            HighlightEveryone.IsOn = Storage.Settings.HighlightEveryone;
            Toasts.IsOn = Storage.Settings.Toasts;

            RespUI_M.Value = Storage.Settings.RespUiM;
            RespUI_L.Value = Storage.Settings.RespUiL;
            RespUI_XL.Value = Storage.Settings.RespUiXl;
            AppBarAtBottom_checkbox.IsChecked = Storage.Settings.AppBarAtBottom;

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
        //    Storage.settings.AutoHideChannels = AutoHideChannels.IsOn;
        //    Storage.settings.AutoHidePeople = AutoHidePeople.IsOn;
            Storage.Settings.HighlightEveryone = HighlightEveryone.IsOn;
            Storage.Settings.Toasts = Toasts.IsOn;

            Storage.Settings.RespUiM = RespUI_M.Value;
            Storage.Settings.RespUiL = RespUI_L.Value;
            Storage.Settings.RespUiXl = RespUI_XL.Value;
            Storage.Settings.AppBarAtBottom = (bool)AppBarAtBottom_checkbox.IsChecked;

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
        private void LockChannelsToggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
           //     AutoHideChannels.IsEnabled = false;
           //     AutoHideChannels.IsOn = false;
            }
            else
            {
           //     AutoHideChannels.IsEnabled = true;
           //     AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
            }

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
            Frame.Navigate(typeof(LockScreen), null);
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
                //This fires when the selected item is DMs
                if(e.ClickedItem.ToString() == "" && ServerList.SelectedIndex == 0)
                {
                    ToggleServerListFull(null, null);
                }
                //And this fires when it's a normal item
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
        private void MessageControl_OnLinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
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
            if (e.Link.StartsWith("@!"))
            {
                string val = e.Link.Remove(0, 2);
                ShowUserDetails(val);
            }
            else if (e.Link.StartsWith("@&"))
            {
                string val = e.Link.Remove(0, 2);
                ShowUserDetails(val);
            }
        }
    }
}
