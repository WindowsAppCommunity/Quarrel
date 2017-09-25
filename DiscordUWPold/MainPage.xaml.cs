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
    
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            //BackgroundGateway

            var licenseInformation = CurrentApp.LicenseInformation;
            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
            {
                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                ShowAds = false;
            }

            //LoadCache();
            LoadMessages();
            LoadSettings();

            if (Storage.Cache.Guilds != null)
            {
                foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.Cache.Guilds)
                {
                    ServerList.Items.Add(GuildRender(guild.Value));
                }
            }

            /*foreach (KeyValuePair<string, CacheModels.User> user in Storage.cache.) 
            {
                //TODO: Friends
            }*/

            if (Storage.Settings.LockChannels)
            {
                DetailToggler.Visibility = Visibility.Collapsed;
            }

            if (Session.Online)
            {
                DownloadGuilds();
                EstablishGateway();
            }
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
            Session.Gateway.DirectMessageChannelCreated += DirectMessageChannelCreated;
            Session.Gateway.DirectMessageChannelDeleted += DirectMessageChannelDeleted;
            Session.Gateway.PresenceUpdated += PresenceUpdated;
            //Session.gateway.TypingStarted += TypingStarted;
            await Session.Gateway.ConnectAsync();
        }
        

        private void LoadGuilds()
        {
            //Loading_Backdrop.Visibility = Visibility.Visible;
            LoadingRing.IsActive = true;

            ServerList.Items.Clear();
            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.Cache.Guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
            }

            DownloadGuilds();
            //Loading_Backdrop.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private async void DownloadGuilds()
        {
            LoadingRing.IsActive = true;
            Storage.Cache.CurrentUser = new CacheModels.User(await Session.GetCurrentUser());

            IEnumerable<SharedModels.UserGuild> guilds = await Session.GetGuilds();

            Storage.Cache.Guilds.Clear();

            foreach (SharedModels.UserGuild guild in guilds)
            {
                Storage.Cache.Guilds.Add(guild.Id, new CacheModels.Guild(guild));
            }

            Username.Text = Storage.Cache.CurrentUser.Raw.Username;
            Discriminator.Text = "#" + Storage.Cache.CurrentUser.Raw.Discriminator;

            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + Storage.Cache.CurrentUser.Raw.Id + "/" + Storage.Cache.CurrentUser.Raw.Avatar + ".jpg"));

            Avatar.Fill = image;

            foreach (KeyValuePair<string, CacheModels.Guild> guild in Storage.Cache.Guilds)
            {
                ServerList.Items.Add(GuildRender(guild.Value));
            }

            Storage.SaveCache();
            LoadingRing.IsActive = false;
        }


        private void OpenServerSplit(object sender, RoutedEventArgs e)
        {
            Servers.IsPaneOpen = !Servers.IsPaneOpen;
        }

        private void TogglePeopleShow(object sender, RoutedEventArgs e)
        {
            Members.IsPaneOpen = !Members.IsPaneOpen;
        }

        /*private void ToggleDetails(object sender, RoutedEventArgs e)
        {
            if (Details_Width.Width == new GridLength(Storage.Settings.DetailsViewSize))
            {
                Details_Width.Width = new GridLength(0);
            } else {
                Details_Width.Width = new GridLength(Storage.Settings.DetailsViewSize);
            }

            if (Storage.Settings.AutoHidePeople)
            {
                MemberListToggle.IsChecked = false;
                Members.IsPaneOpen = false;
            }
        }*/


        private void LoadGuild(object sender, SelectionChangedEventArgs e)
        {
            if (ServerList.SelectedItem != null)
            {
                MessageBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "");

                if (LoadingRing != null)
                {
                    LoadingRing.IsActive = true;
                }

                if (Storage.Settings.AutoHideChannels && Details_Width.Width == new GridLength(0))
                {
                    Details_Width.Width = new GridLength(Storage.Settings.DetailsViewSize);
                    DetailToggler.IsChecked = true;
                }

                Messages.Children.Clear();

                if (MemberList != null)
                {
                    MemberList.Children.Clear();
                }

                if (ServerList.SelectedIndex == 0)
                {
                    PinnedAppButton.Visibility = Visibility.Collapsed;
                    SendMessage.Visibility = Visibility.Collapsed;
                    Tip.Visibility = Visibility.Visible;
                    MuteChannelToggle.Visibility = Visibility.Collapsed;
                    /*if (Members != null)
                    {
                        Members.IsPaneOpen = false;
                    }*/
                    //MemberListToggle.IsChecked = false;
                    //MemberListToggle.Visibility = Visibility.Collapsed;
                    DMs.Visibility = Visibility.Visible;
                    //FriendButton.Visibility = Visibility.Visible;
                    if (Storage.Settings.AutoHideChannels)
                    {
                        DetailToggler.IsChecked = true;
                        Details_Width.Width = new GridLength(Storage.Settings.DetailsViewSize);
                    }
                    TextChnContainer.Visibility = Visibility.Collapsed;

                    foreach (KeyValuePair<string, CacheModels.DmCache> channel in Storage.Cache.DMs)
                    {
                        DMs.Items.Add(ChannelRender(channel.Value));
                    }

                    if (Session.Friends != null)
                    {
                        ListView fulllistview = new ListView();
                        foreach (SharedModels.Friend friend in Session.Friends)
                        {
                            fulllistview.Items.Add(FriendRender(friend));
                        }
                        MemberList.Children.Add(fulllistview);
                    }
                }
                else
                {
                    Permissions perms = new Permissions();

                    foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                    {
                        if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new CacheModels.Member(Session.GetGuildMember((ServerList.SelectedItem as ListViewItem).Tag.ToString(), Storage.Cache.CurrentUser.Raw.Id)));
                        }
                        if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                        {
                            perms.GetPermissions(role, Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles);
                        }
                        else
                        {
                            perms.GetPermissions(0);
                        }
                    }

                    #region Roles

                    List<ListView> listBuffer = new List<ListView>();
                    while (listBuffer.Count < 1000)
                    {
                        listBuffer.Add(new ListView());
                    }

                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles != null)
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Clear();
                        foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                        {
                            if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.ContainsKey(role.Id))
                            {
                                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles[role.Id] = role;
                            } else
                            {
                                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Add(role.Id, role);
                            }

                            if (role.Hoist)
                            {
                                ListView listview = new ListView();
                                listview.Header = role.Name;
                                listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                                listview.SelectionMode = ListViewSelectionMode.None;

                                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                                {
                                    if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                    {
                                        ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                        listview.Items.Add(listviewitem);
                                    }
                                }
                                listBuffer.Insert(1000 - role.Position * 3, listview);
                            }
                        }
                    }

                    foreach (ListView listview in listBuffer)
                    {
                        if (listview.Items.Count != 0)
                        {
                            MemberList.Children.Add(listview);
                        }
                    }

                    ListView fulllistview = new ListView();
                    fulllistview.Header = "Everyone";

                    foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                    {
                        if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new CacheModels.Member(member.Value.Raw));
                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            fulllistview.Items.Add(listviewitem);
                        }
                    }
                    MemberList.Children.Add(fulllistview);

                    #endregion

                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Presences != null)
                    {
                        foreach (SharedModels.Presence presence in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Presences)
                        {
                            if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                            {
                                Session.PrecenseDict.Remove(presence.User.Id);
                            }
                            Session.PrecenseDict.Add(presence.User.Id, presence);
                        }
                    }

                    PinnedAppButton.Visibility = Visibility.Collapsed;
                    MemberListToggle.Visibility = Visibility.Visible;
                    DMs.Visibility = Visibility.Collapsed;
                    FriendButton.Visibility = Visibility.Collapsed;
                    if (Storage.Settings.AutoHideChannels)
                    {
                        DetailToggler.IsChecked = true;
                        Details_Width.Width = new GridLength(Storage.Settings.DetailsViewSize);
                    }
                    TextChnContainer.Visibility = Visibility.Visible;

                    //VoiceChannels.Visibility = Visibility.Visible;

                    MuteChannelToggle.Visibility = Visibility.Collapsed;

                    TextChannels.Items.Clear();
                    foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels)
                    {
                        if (channel.Value.Raw.Type == "text")
                        {
                            TextChannels.Items.Add(ChannelRender(channel.Value, perms));
                        }
                    }

                    if (!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Session.Guild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
                    {
                        AddChannelButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        AddChannelButton.Visibility = Visibility.Visible;
                    }

                }

                if (TextChannels.SelectedIndex == -1)
                {
                    SendMessage.Visibility = Visibility.Collapsed;
                    Tip.Visibility = Visibility.Visible;
                }
                //if (Loading_Backdrop != null)
                //{
                //    Loading_Backdrop.Visibility = Visibility.Collapsed;
                //}
                if (LoadingRing != null)
                {
                    LoadingRing.IsActive = false;
                }

                if (Session.Online)
                {
                    DownloadGuild();
                }
            }
        }

        private async void DownloadGuild()
        {
            if (MemberList != null)
            {
                MemberList.Children.Clear();
            }

            if (ServerList.SelectedIndex == 0)
            {
                Storage.Cache.DMs.Clear();
                foreach (SharedModels.DirectMessageChannel dm in await Session.GetDMs())
                {
                    Storage.Cache.DMs.Add(dm.Id, new CacheModels.DmCache(dm));
                }

                DMs.Items.Clear();
                foreach (KeyValuePair<string, CacheModels.DmCache> dm in Storage.Cache.DMs)
                {
                    DMs.Items.Add(ChannelRender(dm.Value));
                }

            } else
            {
                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild = await Session.GetGuild((ServerList.SelectedItem as ListViewItem).Tag.ToString());

                IEnumerable<SharedModels.GuildMember> members = await Session.GetGuildMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString());

                foreach (SharedModels.GuildMember member in members)
                {
                    if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.User.Id))
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.User.Id, new CacheModels.Member(member));
                    }
                }

                #region Perms
                Permissions perms = new Permissions();
                foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                {
                    if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(Storage.Cache.CurrentUser.Raw.Id))
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(Storage.Cache.CurrentUser.Raw.Id, new CacheModels.Member(Session.GetGuildMember((ServerList.SelectedItem as ListViewItem).Tag.ToString(), Storage.Cache.CurrentUser.Raw.Id)));
                    }
                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Count() != 0 && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.First().ToString() == role.Id)
                    {
                        perms.GetPermissions(role, Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles);
                    }
                    else
                    {
                        perms.GetPermissions(0);
                    }
                }
                #endregion

                #region MemberList

                List<ListView> listBuffer = new List<ListView>();
                while (listBuffer.Count < 1000)
                {
                    listBuffer.Add(new ListView());
                }

                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles != null)
                {
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Clear();
                    foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                    {
                        if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.ContainsKey(role.Id))
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles[role.Id] = role;
                        }
                        else
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Add(role.Id, role);
                        }

                        if (role.Hoist)
                        {
                            ListView listview = new ListView();
                            listview.Header = role.Name;
                            listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                            listview.SelectionMode = ListViewSelectionMode.None;

                            foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                            {
                                if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                {
                                    ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                    listview.Items.Add(listviewitem);
                                }
                            }
                            listBuffer.Insert(1000 - role.Position * 3, listview);
                        }
                    }
                }

                foreach (ListView listview in listBuffer)
                {
                    if (listview.Items.Count != 0)
                    {
                        MemberList.Children.Add(listview);
                    }
                }

                ListView fulllistview = new ListView();
                fulllistview.Header = "Everyone";


                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                {
                    fulllistview.Items.Add((GuildMemberRender(member.Value.Raw) as ListViewItem));
                }
                MemberList.Children.Add(fulllistview);

                #endregion

                #region Presences
                if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Presences != null)
                {
                    foreach (SharedModels.Presence presence in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Presences)
                    {
                        if (Session.PrecenseDict.ContainsKey(presence.User.Id))
                        {
                            Session.PrecenseDict.Remove(presence.User.Id);
                        }
                        Session.PrecenseDict.Add(presence.User.Id, presence);
                    }
                }
                #endregion

                #region Channels

                if ((ServerList.SelectedItem as ListViewItem).Tag != null)
                {
                    IEnumerable<SharedModels.GuildChannel> channels = await Session.GetGuildData((ServerList.SelectedItem as ListViewItem).Tag.ToString());
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels.Clear();
                    foreach (SharedModels.GuildChannel channel in channels)
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels.Add(channel.Id, new CacheModels.GuildChannel(channel));
                    }
                }

                TextChannels.Items.Clear();
                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels)
                {
                    if (channel.Value.Raw.Type == "text")
                    {
                        TextChannels.Items.Add(ChannelRender(channel.Value, perms));
                    }
                }


                if (!perms.EffectivePerms.ManageChannels && !perms.EffectivePerms.Administrator && Session.Guild.OwnerId != Storage.Cache.CurrentUser.Raw.Id)
                {
                    AddChannelButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AddChannelButton.Visibility = Visibility.Visible;
                }
                #endregion

                if ((ServerList.SelectedItem as ListViewItem).Tag.ToString() == "DMs")
                {
                    #region Members
                    Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Clear();
                    foreach (SharedModels.GuildMember member in await Session.GetGuildMembers((ServerList.SelectedItem as ListViewItem).Tag.ToString()))
                    {
                        if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.User.Id))
                        {

                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members[member.User.Id] = new CacheModels.Member(member);
                        }
                        else
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.User.Id, new CacheModels.Member(member));
                        }
                    }

                    List<ListView> dmListBuffer = new List<ListView>();
                    while (dmListBuffer.Count < 1000)
                    {
                        dmListBuffer.Add(new ListView());
                    }

                    if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles != null)
                    {
                        Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Clear();
                        foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
                        {
                            if (Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.ContainsKey(role.Id))
                            {
                                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Roles.Add(role.Id, role);
                            }

                            if (role.Hoist)
                            {
                                ListView listview = new ListView();
                                listview.Header = role.Name;
                                listview.Foreground = GetSolidColorBrush("#FFFFFFFF");
                                listview.SelectionMode = ListViewSelectionMode.None;

                                foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                                {
                                    if (member.Value.Raw.Roles.Contains<string>(role.Id))
                                    {
                                        ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                                        listview.Items.Add(listviewitem);
                                    }
                                }
                                dmListBuffer.Insert(1000 - role.Position * 3, listview);
                            }
                        }


                        foreach (ListView listview in dmListBuffer)
                        {
                            if (listview.Items.Count != 0)
                            {
                                MemberList.Children.Add(listview);
                            }
                        }
                    }

                    ListView dMfulllistview = new ListView();
                    dMfulllistview.Header = "Everyone";

                    foreach (KeyValuePair<string, CacheModels.Member> member in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members)
                    {
                        if (!Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.ContainsKey(member.Value.Raw.User.Id))
                        {
                            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Members.Add(member.Value.Raw.User.Id, new CacheModels.Member(member.Value.Raw));
                            ListViewItem listviewitem = (GuildMemberRender(member.Value.Raw) as ListViewItem);
                            dMfulllistview.Items.Add(listviewitem);
                        }
                    }
                    MemberList.Children.Add(dMfulllistview);
                    #endregion
                }
            }
        }


        private async void PinChannelToStart(object sender, RoutedEventArgs e)
        {
            if (!SecondaryTile.Exists(((sender as Button).Tag as Nullable<SharedModels.GuildChannel>).Value.Id))
            {
                var uriLogo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");

                var currentTime = new DateTime();
                var tileActivationArguments = "timeTileWasPinned=" + currentTime;

                var tile = new Windows.UI.StartScreen.SecondaryTile(((sender as Button).Tag as Nullable<SharedModels.GuildChannel>).Value.Id, ((sender as Button).Tag as Nullable<SharedModels.GuildChannel>).Value.Name, tileActivationArguments, uriLogo, Windows.UI.StartScreen.TileSize.Default);
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;

                bool isCreated = await tile.RequestCreateAsync();
                if (isCreated)
                {
                    MessageDialog msg = new MessageDialog("Pinned Succesfully");
                    await msg.ShowAsync();
                    (sender as Button).Content = "Unpin From Start";
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Failed to Pin");
                    await msg.ShowAsync();
                }
            } else
            {
                var tileToDelete = new SecondaryTile(((sender as Button).Tag as Nullable<SharedModels.GuildChannel>).Value.Id);

                bool isDeleted = await tileToDelete.RequestDeleteAsync();
                if (isDeleted)
                {
                    MessageDialog msg = new MessageDialog("Removed Succesfully");
                    await msg.ShowAsync();
                    (sender as Button).Content = "Pin From Start";
                }
                else
                {
                    MessageDialog msg = new MessageDialog("Failed to Remove");
                    await msg.ShowAsync();
                }
            }   
        }

        private void ClearColor(object sender, TappedRoutedEventArgs e)
        {
            (sender as ListViewItem).Background = GetSolidColorBrush("#00000000");
        }

        private void MuteAChannel(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).Tag != null)
            {
                if (Storage.MutedChannels.Contains((sender as ToggleButton).Tag.ToString()))
                {
                    Storage.MutedChannels.Remove((sender as ToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if (item.Tag.ToString() == (sender as ToggleButton).Tag.ToString())
                        {
                            /*if (Storage.RecentMessages.ContainsKey(item.Tag.ToString()) && Storage.RecentMessages[item.Tag.ToString()] != User.messages.First().Id)
                            {
                                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                                SolidColorBrush accent = new SolidColorBrush(c);
                                accent.Opacity = Storage.settings.NMIOpacity / 100;
                                item.Background = accent;
                            }
                            else
                            {
                                item.Background = GetSolidColorBrush("#00000000");
                            }*/
                            item.Background = GetSolidColorBrush("#00000000");
                        }
                    }
                } else
                {
                    Storage.MutedChannels.Add((sender as ToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if (item.Tag.ToString() == (sender as ToggleButton).Tag.ToString())
                        {
                            SolidColorBrush brush = GetSolidColorBrush("#FFFF0000");
                            brush.Opacity = Storage.Settings.NmiOpacity / 100;
                            item.Background = brush;
                        }
                    }
                }
                Storage.SaveMessages();
            } else if ((sender as AppBarToggleButton).Tag != null)
            {
                if (Storage.MutedChannels.Contains((sender as AppBarToggleButton).Tag.ToString()))
                {
                    Storage.MutedChannels.Remove((sender as AppBarToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if (item.Tag.ToString() == (sender as AppBarToggleButton).Tag.ToString())
                        {
                            /*if (Storage.RecentMessages.ContainsKey(item.Tag.ToString()) && Storage.RecentMessages[item.Tag.ToString()] < User.messages.First().Timestamp.Ticks)
                            {
                                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                                Windows.UI.Color c = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                                SolidColorBrush accent = new SolidColorBrush(c);
                                accent.Opacity = Storage.settings.NMIOpacity / 100;
                                item.Background = accent;
                            } else
                            {
                                item.Background = GetSolidColorBrush("#00000000");
                            }*/
                            item.Background = GetSolidColorBrush("#00000000");
                        }
                    }
                }
                else
                {
                    Storage.MutedChannels.Add((sender as AppBarToggleButton).Tag.ToString());
                    foreach (ListViewItem item in TextChannels.Items)
                    {
                        if (item.Tag.ToString() == (sender as AppBarToggleButton).Tag.ToString())
                        {
                            item.Background = GetSolidColorBrush("#11FF0000");
                        }
                    }
                }
                Storage.SaveMessages();
            }
        }


        private void LoadChannelMessages(object sender, SelectionChangedEventArgs e)
        {
            if (TextChannels.SelectedIndex != -1)
            {
                DMs.Visibility = Visibility.Collapsed;
                if (Storage.Settings.AutoHideChannels)
                {
                    Details_Width.Width = new GridLength(0);
                    DetailToggler.IsChecked = false;
                }
                //Loading_Backdrop.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
                PinnedAppButton.Visibility = Visibility.Visible;
                SendMessage.Visibility = Visibility.Visible;
                MuteChannelToggle.Tag = ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id;
                MuteChannelToggle.IsChecked = Storage.MutedChannels.Contains(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);
                MuteChannelToggle.Visibility = Visibility.Visible;
                Tip.Visibility = Visibility.Collapsed;


                Messages.Children.Clear();
                ListViewItem loadmsgs = new ListViewItem();
                loadmsgs.Content = "Load more messages";
                loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
                loadmsgs.Tapped += LoadMoreMessages;
                loadmsgs.Visibility = Visibility.Collapsed;
                Messages.Children.Add(loadmsgs);

                int adCheck = 5;

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages)
                {
                    adCheck--;
                    Messages.Children.Insert(1, MessageRender(message.Value.Raw));
                    if (adCheck == 0 && ShowAds)
                    {
                        StackPanel adstack = new StackPanel();
                        adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        adstack.Children.Add(ad);
                        Messages.Children.Insert(1, adstack);
                        adCheck = 5;
                    }
                }

                PinnedMessages.Items.Clear();

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].PinnedMessages)
                {
                    adCheck--;
                    PinnedMessages.Items.Insert(0, MessageRender(message.Value.Raw));
                    if (adCheck == 0 && ShowAds)
                    {
                        StackPanel adstack = new StackPanel();
                        adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        adstack.Children.Add(ad);
                        PinnedMessages.Items.Insert(1, adstack);
                        adCheck = 5;
                    }
                }

                foreach (KeyValuePair<string, CacheModels.GuildChannel> channel in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels)
                {
                    if (channel.Value.Raw.Id == ((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id)
                    {
                        ChannelName.Text = "#" + channel.Value.Raw.Name;

                        if (channel.Value.Raw.Topic != null)
                        {
                            ChannelTopic.Text = channel.Value.Raw.Topic;
                        }
                        else
                        {
                            ChannelTopic.Text = "";
                        }
                    }
                }

                //Loading_Backdrop.Visibility = Visibility.Collapsed;
                LoadingRing.IsActive = false;
                DownloadChannelMessages();
            }
        }

        private async void DownloadChannelMessages()
        {
            Messages.Children.Clear();
            ListViewItem loadmsgs = new ListViewItem();
            loadmsgs.Content = "Load more messages";
            loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
            loadmsgs.Tapped += LoadMoreMessages;
            loadmsgs.Visibility = Visibility.Collapsed;
            Messages.Children.Add(loadmsgs);

            IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);

            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Clear();

            foreach (SharedModels.Message message in messages)
            {
                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages.Add(message.Id, new CacheModels.Message(message));
            }

            int adCheck = 5;

            foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages)
            {
                adCheck--;
                Messages.Children.Insert(1, MessageRender(message.Value.Raw));
                if (adCheck == 0 && ShowAds)
                {
                    StackPanel adstack = new StackPanel();
                    adstack.Orientation = Orientation.Horizontal;

                    TextBlock txt = new TextBlock();
                    txt.Text = "Ad:";
                    adstack.Children.Add(txt);
                    AdControl ad = new AdControl();
                    ad.HorizontalAlignment = HorizontalAlignment.Left;
                    ad.Width = 300;
                    ad.Height = 50;
                    ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    ad.AdUnitId = "336795";
                    ad.Tag = "Ad";
                    adstack.Children.Add(ad);
                    Messages.Children.Insert(1, adstack);
                    adCheck = 5;
                }
            }


            PinnedMessages.Items.Clear();
            await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);


            IEnumerable<SharedModels.Message> pinnedmessages = await Session.GetChannelPinnedMessages(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id);

            Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].PinnedMessages.Clear();

            foreach (SharedModels.Message message in pinnedmessages)
            {
                Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].PinnedMessages.Add(message.Id, new CacheModels.Message(message));
            }

            adCheck = 5;

            foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].PinnedMessages)
            {
                adCheck--;
                PinnedMessages.Items.Insert(0, MessageRender(message.Value.Raw));
                if (adCheck == 0 && ShowAds)
                {
                    StackPanel adstack = new StackPanel();
                    adstack.Orientation = Orientation.Horizontal;

                    TextBlock txt = new TextBlock();
                    txt.Text = "Ad:";
                    adstack.Children.Add(txt);
                    AdControl ad = new AdControl();
                    ad.HorizontalAlignment = HorizontalAlignment.Left;
                    ad.Width = 300;
                    ad.Height = 50;
                    ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                    ad.AdUnitId = "336795";
                    ad.Tag = "Ad";
                    adstack.Children.Add(ad);
                    PinnedMessages.Items.Insert(1, adstack);
                    adCheck = 5;
                }
            }

            if (TextChannels.SelectedItem != null && Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].Channels[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id].Messages != null)
            {
                if (Storage.RecentMessages.ContainsKey(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id))
                {
                    Storage.RecentMessages[((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id] = ((Messages.Children.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id;
                }
                else
                {
                    Storage.RecentMessages.Add(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id, ((Messages.Children.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id);
                }
            }
            Storage.SaveMessages();
            Storage.SaveCache();
        }


        private async void LoadMoreMessages(object sender, TappedRoutedEventArgs e)
        {
            IEnumerable<SharedModels.Message> msgs;
            if (ServerList.SelectedIndex == 0)
            {
                msgs = await Session.GetChannelMessagesBefore((DMs.SelectedItem as ListViewItem).Tag.ToString(), (Messages.Children[1] as ListViewItem).Tag.ToString());
            }
            else
            {
                msgs = await Session.GetChannelMessagesBefore(((TextChannels.SelectedItem as ListViewItem).Tag as CacheModels.GuildChannel).Raw.Id, (Messages.Children[1] as ListViewItem).Tag.ToString());
            }

            if (msgs != null)
            {
                int adCheck = 1;
                foreach (SharedModels.Message msg in msgs)
                {
                    adCheck--;
                    Messages.Children.Insert(1, MessageRender(msg));
                    if (adCheck == 0 && ShowAds)
                    {
                        StackPanel adstack = new StackPanel();
                        adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        adstack.Children.Add(ad);
                        Messages.Children.Insert(1, adstack);
                    }
                }
            }
            else
            {
                MessageDialog msg = new MessageDialog("Task failed, please try again");
                await msg.ShowAsync();
            }
        }


        private void ShowPinnedFlyout(object sender, RoutedEventArgs e)
        {
            if (PinnedPopup.Visibility == Visibility.Visible)
            {
                PinnedPopup.Visibility = Visibility.Collapsed;
            } else
            {
                PinnedPopup.Visibility = Visibility.Visible;
            }
        }

        private async void GetDmData(object sender, SelectionChangedEventArgs e)
        {
            //Loading_Backdrop.Visibility = Visibility.Visible;
            LoadingRing.IsActive = true;

            //await User.gateway.ConnectAsync();

            if (DMs.SelectedIndex != -1)
            {
                SendMessage.Visibility = Visibility.Visible;
                Tip.Visibility = Visibility.Collapsed;
                MuteChannelToggle.Visibility = Visibility.Collapsed;
                if (Storage.Settings.AutoHideChannels)
                {
                    DetailToggler.IsChecked = false;
                    Details_Width.Width = new GridLength(0);
                }
                Messages.Children.Clear();
                int adCheck = 5;
                ListViewItem loadmsgs = new ListViewItem();
                loadmsgs.Content = "Load more messages";
                loadmsgs.HorizontalAlignment = HorizontalAlignment.Stretch;
                //loadmsgs.Visibility = Visibility.Collapsed;
                loadmsgs.Tapped += LoadMoreMessages;
                Messages.Children.Add(loadmsgs);

                IEnumerable<SharedModels.Message> messages = await Session.GetChannelMessages(((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id);

                foreach (SharedModels.Message message in messages)
                {
                    Storage.Cache.DMs[((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages.Add(message.Id, new CacheModels.Message(message));
                }

                foreach (KeyValuePair<string, CacheModels.Message> message in Storage.Cache.DMs[((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages)
                {
                    adCheck--;
                    Messages.Children.Insert(1, MessageRender(message.Value.Raw));
                    if (adCheck == 0 && ShowAds)
                    {
                        StackPanel adstack = new StackPanel();
                        adstack.Orientation = Orientation.Horizontal;

                        TextBlock txt = new TextBlock();
                        txt.Text = "Ad:";
                        adstack.Children.Add(txt);
                        AdControl ad = new AdControl();
                        ad.HorizontalAlignment = HorizontalAlignment.Left;
                        ad.Width = 300;
                        ad.Height = 50;
                        ad.ApplicationId = "d9818ea9-2456-4e67-ae3d-01083db564ee";
                        ad.AdUnitId = "336795";
                        ad.Tag = "Ad";
                        adstack.Children.Add(ad);
                        Messages.Children.Insert(1, adstack);
                        adCheck = 5;
                    }
                }

                ChannelName.Text = "@" + Storage.Cache.DMs[((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Raw.User.Username;

                /*foreach (KeyValuePair<string, CacheModels.DMCache> channel in Storage.cache.DMs)
                {
                    if (channel.Key== (((DMs.SelectedItem as ListViewItem).Tag as Nullable<SharedModels.DirectMessageChannel>).Value.Id)
                    {
                        ChannelName.Text = "@" + channel.Value.raw.User.Username;
                        ChannelTopic.Text = "";
                    }
                }*/
            }

            if (DMs.SelectedItem != null && Storage.Cache.DMs[((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id].Messages != null)
            {
                if (Storage.RecentMessages.ContainsKey(((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id))
                {
                    Storage.RecentMessages[((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw
                        .Id] = ((Messages.Children.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id;
                }
                else
                {
                    Storage.RecentMessages.Add(((DMs.SelectedItem as ListViewItem).Tag as CacheModels.DmCache).Raw.Id, ((Messages.Children.Last() as ListViewItem).Tag as Nullable<SharedModels.Message>).Value.Id);
                }
            }
            //Loading_Backdrop.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private void OpenUserSettings(object sender, RoutedEventArgs e)
        {
            /*EmailChange.PlaceholderText = User.CurrentUser.Email;
            UsernameChange.PlaceholderText = User.CurrentUser.Username;*/
            LockChannelList.IsOn = Storage.Settings.LockChannels;
            if (Storage.Settings.LockChannels)
            {
                AutoHideChannels.IsEnabled = false;
            }
            AutoHideChannels.IsOn = Storage.Settings.AutoHideChannels;
            AutoHidePeople.IsOn = Storage.Settings.AutoHidePeople;
            HighlightEveryone.IsOn = Storage.Settings.HighlightEveryone;
            Toasts.IsOn = Storage.Settings.Toasts;
            UserSettings.Visibility = Visibility.Visible;
        }

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
            _settingsChannel = (sender as Button).Tag.ToString();
            ChannelSettings.Visibility = Visibility.Visible;
        }

        private async void OpenGuildSettings(object sender, RoutedEventArgs e)
        {
            //Loading_Backdrop.Visibility = Visibility.Visible;
            LoadingRing.IsActive = true;
            await Session.GetGuild((sender as Button).Tag.ToString());
            SharedModels.Guild guild = Session.Guild;
            ServerNameChange.Text = guild.Name;
            ServerNameChange.PlaceholderText = guild.Name;
            _settingsChannel = (sender as Button).Tag.ToString();
            GuildSettings.Visibility = Visibility.Visible;
            //Loading_Backdrop.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }

        private void CloseUserSettings(object sender, RoutedEventArgs e)
        {
            UserSettings.Visibility = Visibility.Collapsed;
        }

        private void CloseChannelSettings(object sender, RoutedEventArgs e)
        {
            ChannelSettings.Visibility = Visibility.Collapsed;
        }

        private void CloseGuildSettings(object sender, RoutedEventArgs e)
        {
            GuildSettings.Visibility = Visibility.Collapsed;
        }

        private void SaveUserSettings(object sender, RoutedEventArgs e)
        {
            UserSettings.Visibility = Visibility.Collapsed;

            Storage.Settings.AutoHideChannels = AutoHideChannels.IsOn;
            Storage.Settings.AutoHidePeople = AutoHidePeople.IsOn;
            Storage.Settings.HighlightEveryone = HighlightEveryone.IsOn;
            Storage.Settings.Toasts = Toasts.IsOn;
            Storage.Settings.DetailsViewSize = DetailsSize.Value;
            Storage.Settings.NmiOpacity = NMIOp.Value;
            Storage.Settings.LockChannels = LockChannelList.IsOn;
            if (LockChannelList.IsOn)
            {
                DetailToggler.Visibility = Visibility.Collapsed;
                Details_Width.Width = new GridLength(Storage.Settings.DetailsViewSize);
            } else
            {
                DetailToggler.Visibility = Visibility.Visible;
            }
            Storage.SaveAppSettings();
        }

        private void SaveChannelSettings(object sender, RoutedEventArgs e)
        {
            ChannelSettings.Visibility = Visibility.Collapsed;
            Session.ModifyGuildChannel(_settingsChannel, ChannelNameChange.Text, ChannelTopicChnage.Text);
        }

        private void SaveGuildSettings(object sender, RoutedEventArgs e)
        {
            GuildSettings.Visibility = Visibility.Collapsed;
            Session.ModifyGuild(_settingsChannel, ServerNameChange.Text);
        }


        private void Logout(object sender, RoutedEventArgs e)
        {
            Storage.Clear();
            Session.Logout();
            this.Frame.Navigate(typeof(LockScreen));
        }


        string _settingsChannel;

        private void ChangeUserSettingsMenu(object sender, SelectionChangedEventArgs e)
        {
            if (UserAccount_Menu != null)
            {
                switch ((sender as ListView).SelectedIndex)
                {
                    case 0:
                        UserAccount_Menu.Visibility = Visibility.Visible;
                        UI_Menu.Visibility = Visibility.Collapsed;
                        UserNotifications_Menu.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        UserAccount_Menu.Visibility = Visibility.Collapsed;
                        UI_Menu.Visibility = Visibility.Visible;
                        UserNotifications_Menu.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        UserAccount_Menu.Visibility = Visibility.Collapsed;
                        UI_Menu.Visibility = Visibility.Collapsed;
                        UserNotifications_Menu.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void ChangeGuildSettingsMenu(object sender, SelectionChangedEventArgs e)
        { 
            if (GuildOverview_Menu != null)
            {
                switch ((sender as ListView).SelectedIndex)
                {
                    case 0:
                        GuildOverview_Menu.Visibility = Visibility.Visible;
                        GuildRoles_Menu.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        GuildOverview_Menu.Visibility = Visibility.Collapsed;
                        RoleList.Items.Clear();
                        foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
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
                        GuildRoles_Menu.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void ChangeChannelSettingsMenu(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelOverview_Menu != null)
            {
                switch ((sender as ListView).SelectedIndex)
                {
                    case 0:
                        ChannelOverview_Menu.Visibility = Visibility.Visible;
                        ChannelFriends_Menu.Visibility = Visibility.Collapsed;
                        ChannelNotifications_Menu.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void CloseProfPop(object sender, RoutedEventArgs e)
        {
            ProfilePopup.Visibility = Visibility.Collapsed;
        }

        private void OpenWhatNew(object sender, RoutedEventArgs e)
        {
            if (WhatNew.Visibility == Visibility.Visible)
            {
                WhatNew.Visibility = Visibility.Collapsed;
            } else
            {
                WhatNew.Visibility = Visibility.Visible;
            }
        }

        private void CloseWhatNew(object sender, RoutedEventArgs e)
        {
            WhatNew.Visibility = Visibility.Collapsed;
            WhatNewButton.IsChecked = false;
        }

        private void CloseChannelCreate(object sender, RoutedEventArgs e)
        {
            CreateChannel.Visibility = Visibility.Collapsed;
        }

        private async void OpenTwitter(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://twitter.com/AvishaiDernis"));
        }

        private async void OpenDiscordWeb(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://discord.gg/HWTEfjW"));
        }

        private void SwitchEditRoleView(object sender, SelectionChangedEventArgs e)
        {
            RolePermissionsList.Visibility = Visibility.Visible;
            Permissions perms = new Permissions();
            bool manageable = false;
            foreach (SharedModels.Role role in Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles)
            {
                if (role.Id == ((sender as ListView).SelectedItem as ListViewItem).Tag.ToString())
                {
                    perms.GetPermissions(role, Storage.Cache.Guilds[(ServerList.SelectedItem as ListViewItem).Tag.ToString()].RawGuild.Roles);
                    /*foreach (SharedModels.Role userrole in User.guild.Roles)
                    {
                        if (User.memberdict[User.CurrentUser.Id].Roles.First() == userrole.Id)
                        {
                            if (role.Position >= userrole.Position)
                            {
                                Manageable = false;
                            } else
                            {
                                Manageable = true;
                            }
                        }
                    }*/
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

        public static bool ShowAds = true;

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

        private async void OpenFeedback(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        private void Scrolldown(object sender, SizeChangedEventArgs e)
        {
            MessageScroller.ChangeView(0.0f, MessageScroller.ScrollableHeight, 1f);
        }

        private void ShowPurchesesPopup(object sender, RoutedEventArgs e)
        {
            PurchasesPopup.Visibility = Visibility.Visible;
        }

        private void ClosePurchasesPopup(object sender, RoutedEventArgs e)
        {
            PurchasesPopup.Visibility = Visibility.Collapsed;
        }

        private void LockChannelsToggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
                AutoHideChannels.IsEnabled = false;
                AutoHideChannels.IsOn = false;
            } else
            {
                AutoHideChannels.IsEnabled = true;
                AutoHideChannels.IsOn = Storage.Settings.AutoHideChannels;
            }
            
        }
    }
}
