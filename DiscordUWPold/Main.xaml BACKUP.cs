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
            LoadSettings();
            LoadMutedChannels();

            LoadUser();
            LoadGuilds();

            if (Storage.settings.LockChannels)
            {
                ChannelListToggle.Visibility = Visibility.Collapsed;
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
            }

            if (Session.online)
            {
                EstablishGateway();
                var licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                {
                    showAds = false;
                }
            }
            else
            {
                showAds = false;
                FeedbackButton.Visibility = Visibility.Collapsed;
                IAPSButton.Visibility = Visibility.Collapsed;
            }
        }

        public Main(string args)
        {
            this.InitializeComponent();

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
            LoadSettings();
            LoadMutedChannels();

            LoadUser();
            LoadGuilds();

            if (Storage.settings.LockChannels)
            {
                ChannelListToggle.Visibility = Visibility.Collapsed;
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
            }

            if (Session.online)
            {
                EstablishGateway();
                var licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                {
                    showAds = false;
                }
            }
            else
            {
                showAds = false;
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
                    selectGuildId += c;
                }
                else
                {
                    selectChannelId += c;
                }
            }
            selectChannel = true;
        }

        async void EstablishGateway()
        {
            Session.gateway.Ready += OnReady;
            Session.gateway.MessageCreated += MessageCreated;
            Session.gateway.MessageDeleted += MessageDeleted;
            Session.gateway.MessageUpdated += MessageUpdated;

            Session.gateway.GuildCreated += GuildCreated;
            Session.gateway.GuildDeleted += GuildDeleted;
            Session.gateway.GuildUpdated += GuildUpdated;

            Session.gateway.GuildChannelCreated += GuildChannelCreated;
            Session.gateway.GuildChannelDeleted += GuildChannelDeleted;
            Session.gateway.GuildChannelUpdated += GuildChannelUpdated;

            Session.gateway.DirectMessageChannelCreated += DirectMessageChannelCreated;
            Session.gateway.DirectMessageChannelDeleted += DirectMessageChannelDeleted;

            Session.gateway.PresenceUpdated += PresenceUpdated;
            Session.gateway.TypingStarted += TypingStarted;
            await Session.gateway.ConnectAsync();
        }

        #region LoadUser
        private void LoadUser()
        {
            if (Storage.cache.currentUser != null)
            {
                Username.Text = Storage.cache.currentUser.raw.Username;
                Discriminator.Text = "#" + Storage.cache.currentUser.raw.Discriminator;
            }

            if (Session.online)
            {
                DownloadUser();
            } else
            {
                //disable online functions
            }
        }
        private async void DownloadUser()
        {
            Storage.cache.currentUser = new CacheModels.User(await Session.GetCurrentUser());

            Username.Text = Storage.cache.currentUser.raw.Username;
            Discriminator.Text = "#" + Storage.cache.currentUser.raw.Discriminator;

            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + Storage.cache.currentUser.raw.Id + "/" + Storage.cache.currentUser.raw.Avatar + ".jpg"));
            Avatar.Fill = image;
            Storage.SaveCache();
        }
        #endregion

        #region LoadGuilds
        private async void LoadGuilds()
        {
            ServerList.Items.Clear();
            ServerList.Items.Add(MakeDMIcon());
            ServerList.SelectedIndex = 0;
            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.cache.guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
            }
            if (Session.online)
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
                if (Storage.cache.guilds.ContainsKey(guild.Id))
                {
                    var channels = Storage.cache.guilds[guild.Id].channels;
                    var members = Storage.cache.guilds[guild.Id].members;
                    Storage.cache.guilds[guild.Id] = new CacheModels.Guild(guild);
                    Storage.cache.guilds[guild.Id].rawGuild = await Session.GetGuild(guild.Id);
                    Storage.cache.guilds[guild.Id].channels = channels;
                    Storage.cache.guilds[guild.Id].members = members;
                }
                else
                {
                    Storage.cache.guilds.Add(guild.Id, new CacheModels.Guild(guild));
                    Storage.cache.guilds[guild.Id].rawGuild = await Session.GetGuild(guild.Id);
                }
            }

            ServerList.Items.Clear();
            ServerList.Items.Add(MakeDMIcon());
            ServerList.SelectedIndex = 0;
            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.cache.guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
            }
            Storage.SaveCache();
            if (selectChannel)
            {
                foreach (ListViewItem guild in ServerList.Items)
                {
                    if (guild.Tag.ToString() == selectGuildId)
                    {
                        ServerList.SelectedItem = guild;
                    }
                }
            }
        }
        private UIElement MakeDMIcon()
        {
            ListViewItem item = new ListViewItem(){ Height = 50, Tag = "DMs" };
            StackPanel stack = new StackPanel() { Orientation = Orientation.Horizontal };
            TextBlock Icon = new TextBlock() { FontFamily = new FontFamily("Segoe MDL2 Assets"), Text = "", FontSize = 22, Padding = new Thickness(0, 0, 15, 0) };
            TextBlock Text = new TextBlock() { Text = "Direct Messages" };
            stack.Children.Add(Icon);
            stack.Children.Add(Text);
            item.Content = stack;
            return item;
        }
        #endregion

        #region LoadGuild
        private async void CatchServerSelection(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null) //Called upon clearing
            {
                SendMessage.Visibility = Visibility.Collapsed;
                if (((sender as ListView).SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                {
                    Channels.Visibility = Visibility.Collapsed;
                    DMs.Visibility = Visibility.Visible;
                    if (Session.online)
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
                    if (Session.online)
                    {
                        ChannelsLoading.IsActive = true;
                        await DownloadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    } else
                    {
                        LoadGuild(((sender as ListView).SelectedItem as ListViewItem).Tag.ToString());
                    }
                    if (selectChannel)
                    {
                        foreach (ListViewItem channel in TextChannels.Items)
                        {
                            if ((channel.Tag as CacheModels.GuildChannel).raw.Id == selectChannelId)
                            {
                                TextChannels.SelectedItem = channel;
                            }
                        }
                        selectChannel = false;
                    }
                }
            }
        }

        #region Guild
        private void LoadGuild(string id)
        {
            if (Storage.cache.guilds[id] != null)
            {
                ChannelsLoading.IsActive = true;
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");

                if (Storage.settings.AutoHideChannels && ChannelListParent.Width == 0)
                {
                    ChannelListParent.Width = Storage.settings.DetailsViewSize;
                    ChannelListToggle.IsChecked = true;
                }

                Messages.Items.Clear();

                if (MemberList != null)
                {
                    MemberList.Children.Clear();
                }

                #region Permissions
                Permissions perms = new Permissions();
                if (Storage.cache.guilds[id].rawGuild.Roles != null)
                {
                    foreach (SharedModels.Role role in Storage.cache.guilds[id].rawGuild.Roles)
                    {
                        if (Storage.cache.guilds[id].members.ContainsKey(Storage.cache.currentUser.raw.Id))
                        {
                            if (Storage.cache.guilds[id].members[Storage.cache.currentUser.raw.Id].raw.Roles.Count() != 0 && Storage.cache.guilds[id].members[Storage.cache.currentUser.raw.Id].raw.Roles.First().ToString() == role.Id)
                            {
                                perms.GetPermissions(role, Storage.cache.guilds[id].rawGuild.Roles);
                            }
                            else
                            {
                                perms.GetPermissions(0);
                            }
                        }
                    }
                }
                #endregion

                if ((!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Session.guild.OwnerId != Storage.cache.currentUser.raw.Id) || !Session.online)
                {
                    AddChannelButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AddChannelButton.Visibility = Visibility.Visible;
                }

                #region Roles
                List<ListView> MemberListBuffer = new List<ListView>();
                while (MemberListBuffer.Count < 1000)
                {
                    MemberListBuffer.Add(new ListView());
                }

                if (Storage.cache.guilds[id].rawGuild.Roles != null)
                {
                    Storage.cache.guilds[id].roles.Clear();
                    foreach (SharedModels.Role role in Storage.cache.guilds[id].rawGuild.Roles)
                    {
                        if (Storage.cache.guilds[id].roles.ContainsKey(role.Id))
                        {
                            Storage.cache.guilds[id].roles[role.Id] = role;
                        }
                        else
                        {
                            Storage.cache.guilds[id].roles.Add(role.Id, role);
                        }

                        if (role.Hoist)
                        {
                            ListView listview = new ListView();
                            listview.Header = role.Name;
                            listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                            listview.SelectionMode = ListViewSelectionMode.None;

                            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.cache.guilds[id].members)
                            {
                                if (member.Value.raw.Roles.Contains<string>(role.Id))
                                {
                                    ListViewItem listviewitem = (GuildMemberRender(member.Value.raw) as ListViewItem);
                                    listview.Items.Add(listviewitem);
                                }
                            }
                            MemberListBuffer.Insert(1000 - role.Position * 3, listview);
                        }
                    }
                }

                foreach (ListView listview in MemberListBuffer)
                {
                    if (listview.Items.Count != 0)
                    {
                        MemberList.Children.Add(listview);
                    }
                }

                ListView fulllistview = new ListView();
                fulllistview.Header = "Everyone";

                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.cache.guilds[id].members)
                {
                    if (Storage.cache.guilds[id].members.ContainsKey(member.Value.raw.User.Id))
                    {
                        ListViewItem listviewitem = (GuildMemberRender(member.Value.raw) as ListViewItem);
                        fulllistview.Items.Add(listviewitem);
                    }
                }
                MemberList.Children.Add(fulllistview);

                #endregion

                if (Storage.cache.guilds[id].rawGuild.Presences != null)
                {
                    foreach (SharedModels.Presence presence in Storage.cache.guilds[id].rawGuild.Presences)
                    {
                        if (Session.precenseDict.ContainsKey(presence.User.Id))
                        {
                            Session.precenseDict.Remove(presence.User.Id);
                        }
                        Session.precenseDict.Add(presence.User.Id, presence);
                    }
                }

                if (Storage.settings.AutoHideChannels)
                {
                    ChannelListToggle.IsChecked = true;
                    ChannelListParent.Width = Storage.settings.DetailsViewSize;
                }

                List<UIElement> ChannelListBuffer = new List<UIElement>();
                while (ChannelListBuffer.Count < 1000)
                {
                    ChannelListBuffer.Add(new Grid());
                }

                TextChannels.Items.Clear();
                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.cache.guilds[id].channels)
                {
                    if (channel.Value.raw.Type == "text")
                    {
                        ChannelListBuffer.RemoveAt(channel.Value.raw.Position);
                        ChannelListBuffer.Insert(channel.Value.raw.Position, ChannelRender(channel.Value, perms));
                    }
                }

                foreach (UIElement element in ChannelListBuffer)
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
        }
        private async Task DownloadGuild(string id)
        {
            Storage.cache.guilds[id].rawGuild = await Session.GetGuild(id);

            IEnumerable<SharedModels.GuildMember> members = await Session.GetGuildMembers(id);

            foreach (SharedModels.GuildMember member in members)
            {
                if (!Storage.cache.guilds[id].members.ContainsKey(member.User.Id))
                {
                    Storage.cache.guilds[id].members.Add(member.User.Id, new CacheModels.Member(member));
                }
            }

            if (Storage.settings.AutoHideChannels && ChannelListParent.Width == 0)
            {
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
                ChannelListToggle.IsChecked = true;
            }

            Messages.Items.Clear();

            if (MemberList != null)
            {
                MemberList.Children.Clear();
            }

            #region Permissions
            Permissions perms = new Permissions();
            foreach (SharedModels.Role role in Storage.cache.guilds[id].rawGuild.Roles)
            {
                if (!Storage.cache.guilds[id].members.ContainsKey(Storage.cache.currentUser.raw.Id))
                {
                    Storage.cache.guilds[id].members.Add(Storage.cache.currentUser.raw.Id, new CacheModels.Member(Session.GetGuildMember(id, Storage.cache.currentUser.raw.Id)));
                }
                if (Storage.cache.guilds[id].members[Storage.cache.currentUser.raw.Id].raw.Roles.Count() != 0 && Storage.cache.guilds[id].members[Storage.cache.currentUser.raw.Id].raw.Roles.First().ToString() == role.Id)
                {
                    perms.GetPermissions(role, Storage.cache.guilds[id].rawGuild.Roles);
                }
                else
                {
                    perms.GetPermissions(0);
                }
            }
            #endregion

            if (!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Session.guild.OwnerId != Storage.cache.currentUser.raw.Id)
            {
                AddChannelButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddChannelButton.Visibility = Visibility.Visible;
            }

            #region Roles
            List<ListView> MemberListBuffer = new List<ListView>();
            while (MemberListBuffer.Count < 1000)
            {
                MemberListBuffer.Add(new ListView());
            }

            if (Storage.cache.guilds[id].rawGuild.Roles != null)
            {
                Storage.cache.guilds[id].roles.Clear();
                foreach (SharedModels.Role role in Storage.cache.guilds[id].rawGuild.Roles)
                {
                    if (Storage.cache.guilds[id].roles.ContainsKey(role.Id))
                    {
                        Storage.cache.guilds[id].roles[role.Id] = role;
                    }
                    else
                    {
                        Storage.cache.guilds[id].roles.Add(role.Id, role);
                    }

                    if (role.Hoist)
                    {
                        ListView listview = new ListView();
                        listview.Header = role.Name;
                        listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                        listview.SelectionMode = ListViewSelectionMode.None;

                        foreach (KeyValuePair<string, CacheModels.Member> member in Storage.cache.guilds[id].members)
                        {
                            if (member.Value.raw.Roles.Contains<string>(role.Id))
                            {
                                ListViewItem listviewitem = (GuildMemberRender(member.Value.raw) as ListViewItem);
                                listview.Items.Add(listviewitem);
                            }
                        }
                        MemberListBuffer.Insert(1000 - role.Position * 3, listview);
                    }
                }
            }

            foreach (ListView listview in MemberListBuffer)
            {
                if (listview.Items.Count != 0)
                {
                    MemberList.Children.Add(listview);
                }
            }

            ListView fulllistview = new ListView();
            fulllistview.Header = "Everyone";
            fulllistview.SelectionMode = ListViewSelectionMode.None;

            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].members)
            {
                if (Storage.cache.guilds[id].members.ContainsKey(member.Value.raw.User.Id))
                {
                    ListViewItem listviewitem = (GuildMemberRender(member.Value.raw) as ListViewItem);
                    fulllistview.Items.Add(listviewitem);
                }
            }
            MemberList.Children.Add(fulllistview);

            #endregion

            if (Storage.cache.guilds[id].rawGuild.Presences != null)
            {
                foreach (SharedModels.Presence presence in Storage.cache.guilds[id].rawGuild.Presences)
                {
                    if (Session.precenseDict.ContainsKey(presence.User.Id))
                    {
                        Session.precenseDict.Remove(presence.User.Id);
                    }
                    Session.precenseDict.Add(presence.User.Id, presence);
                }
            }

            if (Storage.settings.AutoHideChannels)
            {
                ChannelListToggle.IsChecked = true;
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
            }

            #region Channels
            if ((ServerList.SelectedItem as ListViewItem).Tag != null)
            {
                IEnumerable<SharedModels.GuildChannel> channels = await Session.GetGuildData(id);
                Storage.cache.guilds[id].channels.Clear();
                foreach (SharedModels.GuildChannel channel in channels)
                {
                    Storage.cache.guilds[id].channels.Add(channel.Id, new CacheModels.GuildChannel(channel));
                }
            }


            List<UIElement> ChannelListBuffer = new List<UIElement>();
            while (ChannelListBuffer.Count < 1000)
            {
                ChannelListBuffer.Add(new Grid());
            }

            TextChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.cache.guilds[id].channels)
            {
                if (channel.Value.raw.Type == "text")
                {
                    ChannelListBuffer.RemoveAt(channel.Value.raw.Position);
                    ChannelListBuffer.Insert(channel.Value.raw.Position, ChannelRender(channel.Value, perms));
                }
            }

            foreach (UIElement element in ChannelListBuffer)
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
            if (Storage.settings.AutoHideChannels)
            {
                ChannelListToggle.IsChecked = true;
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
            }
            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.DMCache> channel in Storage.cache.DMs)
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
        }
        private async Task DownloadDMs()
        {
            Storage.cache.DMs.Clear();
            foreach (SharedModels.DirectMessageChannel DM in await Session.GetDMs())
            {
                if (!Storage.cache.DMs.ContainsKey(DM.Id))
                {
                    Storage.cache.DMs.Add(DM.Id, new CacheModels.DMCache(DM));
                }
            }

            DirectMessageChannels.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.DMCache> DM in Storage.cache.DMs)
            {
                DirectMessageChannels.Items.Add(ChannelRender(DM.Value));
            }

            DMsLoading.IsActive = false;
        }
        #endregion
        #endregion

        #region LoadChannel
        private async void LoadChannelMessages(object sender, SelectionChangedEventArgs e)
        {
            if (TextChannels.SelectedItem != null) //Called upon clear
            {
                MessagesLoading.IsActive = true;
                if (Storage.settings.AutoHideChannels)
                {
                    ChannelListParent.Width = 0;
                    ChannelListToggle.IsChecked = false;
                }
                SendMessage.Visibility = Visibility.Visible;
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id);
                MuteToggle.Visibility = Visibility.Visible;

                Messages.Items.Clear();
                ListViewItem loadmsgs = new ListViewItem();
                loadmsgs.Content = "Load more messages";
                loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
                //loadmsgs.Tapped += LoadMoreMessages;
                loadmsgs.Visibility = Visibility.Collapsed;
                Messages.Items.Add(loadmsgs);

                int AdCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].messages)
                {
                    AdCheck--;
                    Messages.Items.Insert(1, MessageRender(message.Value.raw));
                    if (AdCheck == 0 && showAds)
                    {
                        StackPanel Adstack = new StackPanel();
                        Adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        Adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        Adstack.Children.Add(ad);
                        Messages.Items.Insert(1, Adstack);
                        AdCheck = 5;
                    }
                }

                PinnedMessages.Children.Clear();

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].pinnedMessages)
                {
                    AdCheck--;
                    PinnedMessages.Children.Insert(0, MessageRender(message.Value.raw));
                    if (AdCheck == 0 && showAds)
                    {
                        StackPanel Adstack = new StackPanel();
                        Adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        Adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        Adstack.Children.Add(ad);
                        PinnedMessages.Children.Insert(1, Adstack);
                        AdCheck = 5;
                    }
                }

                ChannelName.Text = "#" + Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Name;

                if (Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Topic;
                }
                else
                {
                    ChannelTopic.Text = "";
                }
                if (Session.online)
                {
                    await DownloadChannelMessages();
                } else
                {
                    MessagesLoading.IsActive = false;
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
                MuteToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id;
                MuteToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id);
                MuteToggle.Visibility = Visibility.Visible;
                Messages.Items.Clear();
                IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id);

                Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].messages.Clear();

                while (messages == null)
                {
                    //Messages may not be downloaded yet
                }

                foreach (SharedModels.Message message in messages)
                {
                    Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].messages.Add(message.Id, new CacheModels.Message(message));
                }

                ListViewItem loadmsgs = new ListViewItem();
                loadmsgs.Content = "Load more messages";
                loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
                loadmsgs.Tapped += LoadMoreMessages;
                loadmsgs.Tag = messages.First().ChannelId;
                //loadmsgs.Visibility = Visibility.Collapsed;
                Messages.Items.Add(loadmsgs);


                int AdCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].messages)
                {
                    AdCheck--;
                    Messages.Items.Insert(1, MessageRender(message.Value.raw));
                    if (AdCheck == 0 && showAds)
                    {
                        StackPanel Adstack = new StackPanel();
                        Adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        Adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Center;
                        ad.Margin = new Thickness(0, 6, 0, 6);
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        Adstack.Children.Add(ad);
                        Messages.Items.Insert(1, Adstack);
                        AdCheck = 5;
                    }
                }

                PinnedMessages.Children.Clear();
                await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id);

                IEnumerable<SharedModels.Message> pinnedmessages = await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id);

                Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].pinnedMessages.Clear();

                foreach (SharedModels.Message message in pinnedmessages)
                {
                    Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].pinnedMessages.Add(message.Id, new CacheModels.Message(message));
                }


                AdCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].pinnedMessages)
                {
                    AdCheck--;
                    PinnedMessages.Children.Insert(0, MessageRender(message.Value.raw));
                    if (AdCheck == 0 && showAds)
                    {
                        StackPanel Adstack = new StackPanel();
                        Adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        Adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Center;
                        ad.Margin = new Thickness(0, 6, 0, 6);
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        Adstack.Children.Add(ad);
                        PinnedMessages.Children.Insert(1, Adstack);
                        AdCheck = 5;
                    }
                }

                ChannelName.Text = "#" + Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Name;

                if (Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Topic != null)
                {
                    ChannelTopic.Text = Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].raw.Topic;
                }
                else
                {
                    ChannelTopic.Text = "";
                }

                if (TextChannels.SelectedItem != null && Storage.cache.guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id].messages != null)
                {
                    if (Storage.RecentMessages.ContainsKey(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id))
                    {
                        Storage.RecentMessages[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id] = ((Messages.Items.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id;
                    }
                    else
                    {
                        if (((Messages.Items.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).HasValue)
                        {
                            Storage.RecentMessages.Add(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).raw.Id, ((Messages.Items.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id);
                        }
                    }
                    Storage.SaveMessages();
                }
                Storage.SaveCache();

                MessagesLoading.IsActive = false;
            }
        }
        #endregion

        #region LoadDMChannel
        private async void LoadDMChannelMessages(object sender, SelectionChangedEventArgs e)
        {
            if (DirectMessageChannels.SelectedItem != null)
            {
                MessagesLoading.IsActive = true;
                SendMessage.Visibility = Visibility.Visible;
                SendBox.IsEnabled = false;
                MuteToggle.Visibility = Visibility.Collapsed;
                if (Storage.settings.AutoHideChannels)
                {
                    ChannelListToggle.IsChecked = false;
                    ChannelListParent.Width = 0;
                }

                Messages.Items.Clear();
                int AdCheck = 5;
                ListViewItem loadmsgs = new ListViewItem();
                loadmsgs.Content = "Load more messages";
                loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
                loadmsgs.Visibility = Visibility.Collapsed;
                //loadmsgs.Tapped += LoadMoreMessages;
                Messages.Items.Add(loadmsgs);

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].messages)
                {
                    AdCheck--;
                    Messages.Items.Insert(1, MessageRender(message.Value.raw));
                    if (AdCheck == 0 && showAds)
                    {
                        StackPanel Adstack = new StackPanel();
                        Adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        Adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Center;
                        ad.Margin = new Thickness(0, 6, 0, 6);
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        Adstack.Children.Add(ad);
                        Messages.Items.Insert(1, Adstack);
                        AdCheck = 5;
                    }
                }

                ChannelName.Text = "@" + Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].raw.User.Username;
                ChannelTopic.Text = "";

                if (Session.online)
                {
                    await DownloadDMChannelMessages();
                } else
                {
                    MessagesLoading.IsActive = false;
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
        private async Task DownloadDMChannelMessages()
        {
            SendMessage.Visibility = Visibility.Visible;
            MuteToggle.Visibility = Visibility.Collapsed;
            if (Storage.settings.AutoHideChannels)
            {
                ChannelListToggle.IsChecked = false;
                ChannelListParent.Width = 0;
            }

            Messages.Items.Clear();
            int AdCheck = 5;
            ListViewItem loadmsgs = new ListViewItem();
            loadmsgs.Content = "Load more messages";
            loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
            loadmsgs.Visibility = Visibility.Collapsed;
            //loadmsgs.Tapped += LoadMoreMessages;
            Messages.Items.Add(loadmsgs);

            IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id);

            if (Storage.cache.DMs != null)
            {
                Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].messages.Clear();
            }

            foreach (SharedModels.Message message in messages)
            {
                Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].messages.Add(message.Id, new CacheModels.Message(message));
            }

            foreach (KeyValuePair<string, CacheModels.Message> message in Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].messages)
            {
                AdCheck--;
                Messages.Items.Insert(1, MessageRender(message.Value.raw));
                if (AdCheck == 0 && showAds)
                {
                    StackPanel Adstack = new StackPanel();
                    Adstack.Orientation = Orientation.Horizontal;

                    TextBlock txt = new TextBlock();
                    txt.Text = "Ad:";
                    Adstack.Children.Add(txt);
                    AdControl ad = new AdControl();
                    ad.HorizontalAlignment = HorizontalAlignment.Center;
                    ad.Margin = new Thickness(0, 6, 0, 6);
                    ad.Width = 300;
                    ad.Height = 50;
                    ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    ad.AdUnitId = "336795";
                    ad.Tag = "Ad";
                    Adstack.Children.Add(ad);
                    Messages.Items.Insert(1, Adstack);
                    AdCheck = 5;
                }
            }

            if (DirectMessageChannels.SelectedItem != null && Storage.cache.DMs[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id].messages != null)
            {
                if (Storage.RecentMessages.ContainsKey(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id))
                {
                    Storage.RecentMessages[((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw
                        .Id] = ((Messages.Items.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id;
                }
                else
                {
                    Storage.RecentMessages.Add(((DirectMessageChannels.SelectedItem as ListViewItem).Tag as CacheModels.DMCache).raw.Id, ((Messages.Items.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id);
                }
                Storage.SaveMessages();
            }

            MessagesLoading.IsActive = false;
        }
        #endregion


        #region General
        private void ToggleServerListFull(object sender, RoutedEventArgs e)
        {
            Servers.IsPaneOpen = !Servers.IsPaneOpen;
        }
        private void Scrolldown(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height < 10)
            {
                MessageScroller.ChangeView(0.0f, MessageScroller.ScrollableHeight, 1f);
            }
        }
        private void ToggleChanneList(object sender, RoutedEventArgs e)
        {
            if (ChannelListParent.Width == 0)
            {
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
            } else
            {
                ChannelListParent.Width = 0;
            }
        }
        private void TogglePeopleShow(object sender, RoutedEventArgs e)
        {
            Members.IsPaneOpen = !Members.IsPaneOpen;
        }
        private void TogglePinnedShow(object sender, RoutedEventArgs e)
        {
            PinnedMessagesPopup.IsPaneOpen = !PinnedMessagesPopup.IsPaneOpen;
        }
        private async void LoadMoreMessages(object sender, TappedRoutedEventArgs e)
        {
            IEnumerable<SharedModels.Message> newMessages = await Session.GetChannelMessagesBefore((sender as ListViewItem).Tag.ToString(), ((Messages.Items[2] as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id);

            int AdCheck = 5;

            foreach (SharedModels.Message msg in newMessages)
            {
                AdCheck--;
                Messages.Items.Insert(1, MessageRender(msg));
                if (AdCheck == 0 && showAds)
                {
                    StackPanel Adstack = new StackPanel();
                    Adstack.Orientation = Orientation.Horizontal;
                    TextBlock txt = new TextBlock();
                    txt.Text = "";
                    Adstack.Children.Add(txt);
                    AdControl ad = new AdControl();
                    ad.HorizontalAlignment = HorizontalAlignment.Center;
                    ad.Margin = new Thickness(0, 6, 0, 6);
                    ad.Width = 300;
                    ad.Height = 50;
                    ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    ad.AdUnitId = "336795";
                    ad.Tag = "Ad";
                    ad.Background = (SolidColorBrush)App.Current.Resources["DarkBG"];
                    Adstack.Children.Add(ad);
                    Messages.Items.Insert(1, Adstack);
                    AdCheck = 5;
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
            WhatsNew.IsPaneOpen = true;
        }
        private void OpenIAPs(object sender, RoutedEventArgs e)
        {
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
                                showAds = false;
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
                                showAds = false;
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
                                showAds = false;
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
                                showAds = false;
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
                                showAds = false;
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
            SettingsPaneId = (sender as Button).Tag.ToString();
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
            Session.ModifyGuild(SettingsPaneId, ServerNameChange.Text); //TODO: Fix
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
                bool Manageable = false;
                foreach (SharedModels.Role role in Storage.cache.guilds[SettingsPaneId].rawGuild.Roles)
                {
                    if (role.Id == ((sender as ListView).SelectedItem as ListViewItem).Tag.ToString())
                    {
                        perms.GetPermissions(role, Storage.cache.guilds[SettingsPaneId].rawGuild.Roles);
                    }
                }

                RoleAllowAnyoneToMention.Visibility = Visibility.Collapsed;
                RoleHoist.Visibility = Visibility.Collapsed;

                RoleAddReactions.IsChecked = perms.ServerSidePerms.AddReactions;
                if (!Manageable)
                {
                    RoleAddReactions.IsEnabled = false;
                }
                RoleAdmininstrator.IsChecked = perms.ServerSidePerms.Administrator;
                if (!Manageable)
                {
                    RoleAdmininstrator.IsEnabled = false;
                }
                RoleAttachFiles.IsChecked = perms.ServerSidePerms.AttachFiles;
                if (!Manageable)
                {
                    RoleAttachFiles.IsEnabled = false;
                }
                RoleBanMembers.IsChecked = perms.ServerSidePerms.BanMembers;
                if (!Manageable)
                {
                    RoleBanMembers.IsEnabled = false;
                }
                RoleChangeNickname.IsChecked = perms.ServerSidePerms.ChangeNickname;
                if (!Manageable)
                {
                    RoleChangeNickname.IsEnabled = false;
                }
                RoleConnect.IsChecked = perms.ServerSidePerms.Connect;
                if (!Manageable)
                {
                    RoleConnect.IsEnabled = false;
                }
                RoleCreateInstantInvite.IsChecked = perms.ServerSidePerms.CreateInstantInvite;
                if (!Manageable)
                {
                    RoleCreateInstantInvite.IsEnabled = false;
                }
                RoleDeafenMembers.IsChecked = perms.ServerSidePerms.DeafenMembers;
                if (!Manageable)
                {
                    RoleDeafenMembers.IsEnabled = false;
                }
                RoleKickMembers.IsChecked = perms.ServerSidePerms.KickMembers;
                if (!Manageable)
                {
                    RoleKickMembers.IsEnabled = false;
                }
                RoleManageChannels.IsChecked = perms.ServerSidePerms.ManageChannels;
                if (!Manageable)
                {
                    RoleManageChannels.IsEnabled = false;
                }
                RoleManageEmojis.IsChecked = perms.ServerSidePerms.ManageEmojis;
                if (!Manageable)
                {
                    RoleManageEmojis.IsEnabled = false;
                }
                RoleManageGuild.IsChecked = perms.ServerSidePerms.ManangeGuild;
                if (!Manageable)
                {
                    RoleManageGuild.IsEnabled = false;
                }
                RoleManageNicknames.IsChecked = perms.ServerSidePerms.ManageNicknames;
                if (!Manageable)
                {
                    RoleManageNicknames.IsEnabled = false;
                }
                RoleManageRoles.IsChecked = perms.ServerSidePerms.ManageRoles;
                if (!Manageable)
                {
                    RoleManageRoles.IsEnabled = false;
                }
                RoleMentionEveryone.IsChecked = perms.ServerSidePerms.MentionEveryone;
                if (!Manageable)
                {
                    RoleMentionEveryone.IsEnabled = false;
                }
                RoleMoveMembers.IsChecked = perms.ServerSidePerms.MoveMembers;
                if (!Manageable)
                {
                    RoleMoveMembers.IsEnabled = false;
                }
                RoleMuteMembers.IsChecked = perms.ServerSidePerms.MuteMembers;
                if (!Manageable)
                {
                    RoleMuteMembers.IsEnabled = false;
                }
                RoleReadMessageHistory.IsChecked = perms.ServerSidePerms.ReadMessageHistory;
                if (!Manageable)
                {
                    RoleReadMessageHistory.IsEnabled = false;
                }
                RoleSpeak.IsChecked = perms.ServerSidePerms.Speak;
                if (!Manageable)
                {
                    RoleSpeak.IsEnabled = false;
                }
                RoleUseExternalEmojis.IsChecked = perms.ServerSidePerms.UseExternalEmojis;
                if (!Manageable)
                {
                    RoleUseExternalEmojis.IsEnabled = false;
                }
                RoleUseVoiceActivity.IsChecked = perms.ServerSidePerms.UseVad;
                if (!Manageable)
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
            SettingsPaneId = (sender as Button).Tag.ToString();
            ChannelSettings.IsPaneOpen = true;
            if (!Session.online)
            {
                SaveChannelSettingsButton.IsEnabled = false;
            }
        }
        private void SaveChannelSettings(object sender, RoutedEventArgs e)
        {
            Session.ModifyGuildChannel(SettingsPaneId, ChannelNameChange.Text, ChannelTopicChnage.Text);
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
            Session.DeleteChannel(SettingsPaneId);
            ChannelSettings.IsPaneOpen = false;
        }
        #endregion

        #region UserSettings
        private void OpenUserSettings(object sender, RoutedEventArgs e)
        {
            LockChannelList.IsOn = Storage.settings.LockChannels;
            if (Storage.settings.LockChannels)
            {
                AutoHideChannels.IsEnabled = false;
            }
            AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
            AutoHidePeople.IsOn = Storage.settings.AutoHidePeople;
            HighlightEveryone.IsOn = Storage.settings.HighlightEveryone;
            Toasts.IsOn = Storage.settings.Toasts;
            DetailsSize.Value = Storage.settings.DetailsViewSize;
            NMIOp.Value = Storage.settings.NMIOpacity;
            UserSettings.IsPaneOpen = !UserSettings.IsPaneOpen;
        }
        private void SaveUserSettings(object sender, RoutedEventArgs e)
        {
            Storage.settings.AutoHideChannels = AutoHideChannels.IsOn;
            Storage.settings.AutoHidePeople = AutoHidePeople.IsOn;
            Storage.settings.HighlightEveryone = HighlightEveryone.IsOn;
            Storage.settings.Toasts = Toasts.IsOn;
            Storage.settings.DetailsViewSize = DetailsSize.Value;
            Storage.settings.NMIOpacity = NMIOp.Value;
            Storage.settings.LockChannels = LockChannelList.IsOn;
            if (LockChannelList.IsOn)
            {
                ChannelListToggle.Visibility = Visibility.Collapsed;
                ChannelListParent.Width = Storage.settings.DetailsViewSize;
                ChannelListToggle.IsChecked = true;
            }
            else
            {
                ChannelListToggle.Visibility = Visibility.Visible;
            }
            Storage.SaveAppSettings();
            UserSettings.IsPaneOpen = false;
        }
        private void CloseUserSettings(object sender, RoutedEventArgs e)
        {
            UserSettings.IsPaneOpen = false;
        }
        private void LockChannelsToggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
                AutoHideChannels.IsEnabled = false;
                AutoHideChannels.IsOn = false;
            }
            else
            {
                AutoHideChannels.IsEnabled = true;
                AutoHideChannels.IsOn = Storage.settings.AutoHideChannels;
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
            this.Frame.Content = new LockScreen();
        }
        #endregion

        public static bool showAds = true;
        public bool selectChannel = false;
        public string selectChannelId = "";
        public string selectGuildId = "";
        string SettingsPaneId;
    }
}
