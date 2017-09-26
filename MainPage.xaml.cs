using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
            Setup();
        }

        public async void Setup()
        {
            Loading.Show(false);
            if (await LogIn())
            {
                SetupEvents();
                GatewayManager.StartGateway();
            }
            else
            {
                Frame.Navigate(typeof(LogScreen));
            }
        }

        public async Task<bool> LogIn()
        {
            var credntials = Storage.PasswordVault.FindAllByResource("LogIn"); //TODO: Multi-Account
            var creds = credntials.FirstOrDefault();
            creds.RetrievePassword();
            if (await RESTCalls.Login(creds.UserName, creds.Password))
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void SetupEvents()
        {
            //Navigation
            App.NavigateToGuildHandler += App_NavigateToGuildHandler;
            App.NavigateToGuildChannelHandler += App_NavigateToGuildChannelHandler;
            App.NavigateToDMChannelHandler += App_NavigateToDMChannelHandler;
            //SubPages
            App.SubpageClosedHandler += App_SubpageClosedHandler; ;
            App.NavigateToBugReportHandler += App_NavigateToBugReportHandler;
            App.NavigateToChannelEditHandler += App_NavigateToChannelEditHandler;
            App.NavigateToCreateBanHandler += App_NavigateToCreateBanHandler;
            App.NavigateToCreateServerHandler += App_NavigateToCreateServerHandler;
            App.NavigateToDeleteChannelHandler += App_NavigateToDeleteChannelHandler;
            App.NavigateToDeleteServerHandler += App_NavigateToDeleteServerHandler;
            App.NavigateToGuildEditHandler += App_NavigateToGuildEditHandler;
            App.NavigateToJoinServerHandler += App_NavigateToJoinServerHandler;
            App.NavigateToLeaveServerHandler += App_NavigateToLeaveServerHandler;
            App.NavigateToNicknameEditHandler += App_NavigateToNicknameEditHandler;
            App.NavigateToProfileHandler += App_NavigateToProfileHandler;
            App.OpenAttachementHandler += App_OpenAttachementHandler;
            App.NavigateToChannelTopicHandler += App_NavigateToChannelTopicHandler;
            App.NavigateToCreateChannelHandler += App_NavigateToCreateChannelHandler;
            App.NavigateToSettingsHandler += App_NavigateToSettingsHandler;
            App.NavigateToAboutHandler += App_NavigateToAboutHandler;
            App.NavigateToAddServerHandler += App_NavigateToAddServerHandler;
            App.NavigateToMessageEditorHandler += App_NavigateToMessageEditorHandler;
            //Flyouts
            App.MenuHandler += App_MenuHandler;
            //API
            App.CreateMessageHandler += App_CreateMessageHandler;
            App.StartTypingHandler += App_StartTypingHandler;
            App.AddFriendHandler += App_AddFriendHandler;
            App.BlockUserHandler += App_BlockUserHandler;
            App.MarkChannelAsReadHandler += App_MarkChannelAsReadHandler;
            App.MarkGuildAsReadHandler += App_MarkGuildAsReadHandler;
            App.MuteChannelHandler += App_MuteChannelHandler;
            App.MuteGuildHandler += App_MuteGuildHandler;
            App.RemoveFriendHandler += App_RemoveFriendHandler;
            //UpdateUI
            App.ReadyRecievedHandler += App_ReadyRecievedHandler;
            App.TypingHandler += App_TypingHandler;
            App.UpdateUnreadIndicatorsHandler += App_UpdateUnreadIndicatorsHandler;
            //UpdateUI-Messages
            App.MessageCreatedHandler += App_MessageCreatedHandler;
            App.MessageDeletedHandler += App_MessageDeletedHandler;
            App.MessageEditedHandler += App_MessageEditedHandler;
            //UpdateUI-Channels
            App.GuildChannelCreatedHandler += App_GuildChannelCreatedHandler;
            //UpdateUI-Guilds
            App.GuildCreatedHandler += App_GuildCreatedHandler;
            App.GuildChannelDeletedHandler += App_GuildChannelDeletedHandler;

        }

        #region AppEvents

        #region Navigation
        private void App_NavigateToGuildHandler(object sender, App.GuildNavigationArgs e)
        {
            App.CurrentGuildIsDM = e.GuildId == "DMs"; //Could combine...
            if (e.GuildId != "DMs")
            {
                MemberToggle.Visibility = Visibility.Visible;
                if (Page.ActualWidth > 1500)
                {
                    MembersPane.IsPaneOpen = true;
                }

                foreach (GuildManager.SimpleGuild guild in ServerList.Items)
                {
                    if (guild.Id == e.GuildId)
                    {
                        ServerList.SelectedItem = guild;
                    }
                }

                App.CurrentGuildId = e.GuildId;
                RenderGuildChannels();
            } else
            {
                App.CurrentGuildId = null;
                MemberToggle.Visibility = Visibility.Collapsed;
                MembersPane.IsPaneOpen = false;
                RenderDMChannels();
            }

            if (App.CurrentGuildId == null)
            {
                string[] channels = new string[LocalState.DMs.Count];
                for (int x = 0; x < LocalState.DMs.Count; x++)
                {
                    channels[x] = LocalState.DMs.Values.ToList()[x].Id;
                }
                GatewayManager.Gateway.SubscribeToGuild(channels);
            } else
            {
                string[] channels = new string[LocalState.Guilds[App.CurrentGuildId].channels.Count];
                for (int x = 0; x < LocalState.Guilds[App.CurrentGuildId].channels.Count; x++)
                {
                    channels[x] = LocalState.Guilds[App.CurrentGuildId].channels.Values.ToList()[x].raw.Id;
                }
                GatewayManager.Gateway.SubscribeToGuild(channels);
            }
        }
        private void App_NavigateToGuildChannelHandler(object sender, App.GuildChannelNavigationArgs e)
        {
            if (App.CurrentGuildId == e.GuildId)
            {
                App.CurrentChannelId = e.ChannelId;
                RenderMessages();
            }
            else //Out of guild navigation
            {
                //TODO: Out of guild navigation
            }
        }
        private void App_NavigateToDMChannelHandler(object sender, App.DMChannelNavigationArgs e)
        {
            if (e.ChannelId != null) //Nav by ChannelId
            {
                if (App.CurrentGuildIsDM)
                {
                    App.CurrentChannelId = e.ChannelId;
                    RenderMessages();
                } else
                {
                    ServerList.SelectedIndex = 0;
                    App.CurrentChannelId = e.ChannelId;
                    RenderMessages();
                }
            } else //Nav by UserId
            {
                //TODO: Nav by UserId
            }
        }
        #endregion

        #region SubPages
        private void SubFrameNavigator(Type page, object args = null)
        {
            //TOOD: Settings
            //if (Storage.Settings.ExpensiveRender)
            //{
            //    content.Blur(2, 600).Start();
            //}
            content.Blur(2, 600).Start();
            SubFrame.Visibility = Visibility.Visible;
            SubFrame.Navigate(page, args);
        }
        private void App_SubpageClosedHandler(object sender, EventArgs e)
        {
            //TOOD: Settings
            //if (Storage.Settings.ExpensiveRender)
            //{
            //    content.Blur(0, 600).Start();
            //}
            //else
            //{
            //    content.Blur(0, 0).Start();
            //}
            content.Blur(0, 600).Start();
        }

        private void App_NavigateToBugReportHandler(object sender, App.BugReportNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.BugReport), e.Exception);
        }
        private void App_NavigateToChannelEditHandler(object sender, App.ChannelEditNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.EditChannel), e.ChannelId);
        }
        private void App_NavigateToCreateBanHandler(object sender, App.CreateBanNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.CreateBan), e.UserId);
        }
        private void App_NavigateToCreateServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.CreateServer));
        }
        private void App_NavigateToDeleteChannelHandler(object sender, App.DeleteChannelNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.DeleteChannel), e.ChannelId);
        }
        private void App_NavigateToDeleteServerHandler(object sender, App.DeleteServerNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.DeleteServer), e.GuildId);
        }
        private void App_NavigateToGuildEditHandler(object sender, App.GuildEditNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.EditGuild), e.GuildId);
        }
        private void App_NavigateToJoinServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.JoinServer));
        }
        private void App_NavigateToLeaveServerHandler(object sender, App.LeaverServerNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.LeaveServer), e.GuildId);
        }
        private void App_NavigateToNicknameEditHandler(object sender, App.NicknameEditNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.EditNickname), e.UserId);
        }
        private void App_NavigateToProfileHandler(object sender, App.ProfileNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.UserProfile), e.User.Id);
        }
        private void App_OpenAttachementHandler(object sender, SharedModels.Attachment e)
        {
            SubFrameNavigator(typeof(SubPages.PreviewAttachement), e);
        }
        private void App_NavigateToChannelTopicHandler(object sender, App.ChannelTopicNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.ChannelTopic), e.Channel);
        }
        private void App_NavigateToCreateChannelHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.CreateChannel));
        }
        private void App_NavigateToSettingsHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.Settings));
        }
        private void App_NavigateToAboutHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.About));
        }
        private void App_NavigateToAddServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.AddServer));
        }
        private void App_NavigateToMessageEditorHandler(object sender, App.MessageEditorNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.ExtendedMessageEditor), e.Content);
        }
        #endregion

        #region Flyouts
        private void App_MenuHandler(object sender, App.MenuArgs e)
        {
            e.Flyout.ShowAt((sender as UIElement), e.Point);
        }
        #endregion

        #region API
        private async void App_CreateMessageHandler(object sender, App.CreateMessageArgs e)
        {
            await RESTCalls.CreateMessage(e.ChannelId, e.Message);
        }

        private async void App_StartTypingHandler(object sender, App.StartTypingArgs e)
        {
            await RESTCalls.TriggerTypingIndicator(e.ChannelId);
        }

        private void App_AddFriendHandler(object sender, App.AddFriendArgs e)
        {

        }

        private void App_BlockUserHandler(object sender, App.BlockUserArgs e)
        {

        }

        private void App_MarkChannelAsReadHandler(object sender, App.MarkChannelAsReadArgs e)
        {

        }

        private void App_MarkGuildAsReadHandler(object sender, App.MarkGuildAsReadArgs e)
        {

        }

        private void App_MuteChannelHandler(object sender, App.MuteChannelArgs e)
        {

        }

        private void App_MuteGuildHandler(object sender, App.MuteGuildArgs e)
        {

        }

        private void App_RemoveFriendHandler(object sender, App.RemoveFriendArgs e)
        {

        }
        #endregion

        #region UpdateUI

        #region RenderElement
        public void PopulateMessageArea()
        {
            MessageList.Items.Clear();
            SendMessage.Visibility = Visibility.Visible;
            if (Page.ActualWidth <= 500)
            {
                CompressedChannelHeader.Visibility = Visibility.Visible;
            }
        }
        public void ClearMessageArea()
        {
            friendPanel.Visibility = Visibility.Collapsed;
            MessageList.Items.Clear();
            SendMessage.Visibility = Visibility.Collapsed;
            CompressedChannelHeader.Visibility = Visibility.Collapsed;
        }

        public void RenderCurrentUser()
        {
            ImageBrush image = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + LocalState.CurrentUser.Id + "/" + LocalState.CurrentUser.Avatar + ".jpg")) };
            Avatar.Fill = image;
            LargeAvatar.Fill = image;
            Username.Text = LocalState.CurrentUser.Username;
            Discriminator.Text = "#" + LocalState.CurrentUser.Discriminator;
            LargeUsername.Text = Username.Text;
            LargeDiscriminator.Text = Discriminator.Text;
        }

        public void RenderGuilds()
        {
            GuildManager.SimpleGuild DM = new GuildManager.SimpleGuild();
            DM.Id = "DMs";
            //DM.Name = App.GetString("/Main/DirectMessages");
            DM.Name = "Direct Messages";
            DM.IsDM = true;
            foreach (var chn in LocalState.DMs.Values)
                if (LocalState.RPC.ContainsKey(chn.Id))
                {
                    ReadState readstate = LocalState.RPC[chn.Id];
                    DM.NotificationCount += readstate.MentionCount;
                    var StorageChannel = LocalState.DMs[chn.Id];
                    if (StorageChannel.LastMessageId != null &&
                        readstate.LastMessageId != StorageChannel.LastMessageId)
                        DM.IsUnread = true;
                }
            ServerList.Items.Add(DM);

            foreach (var guild in LocalState.Guilds.OrderBy(x => x.Value.Position))
            {
                var sg = new GuildManager.SimpleGuild();
                sg.Id = guild.Value.Raw.Id;
                if (guild.Value.Raw.Icon != null && guild.Value.Raw.Icon != "")
                {
                    sg.ImageURL = "https://discordapp.com/api/guilds/" + guild.Value.Raw.Id + "/icons/" + guild.Value.Raw.Icon + ".jpg";
                }
                else
                {
                    sg.ImageURL = "empty";
                }
                sg.Name = guild.Value.Raw.Name;

                sg.IsMuted = LocalState.GuildSettings.ContainsKey(guild.Key) ? LocalState.GuildSettings[guild.Key].raw.Muted : false;
                sg.IsUnread = false; //Will change if true
                foreach (var chn in guild.Value.channels.Values)
                    if (LocalState.RPC.ContainsKey(chn.raw.Id))
                    {
                        ReadState readstate = LocalState.RPC[chn.raw.Id];
                        sg.NotificationCount += readstate.MentionCount;
                        var StorageChannel = LocalState.Guilds[sg.Id].channels[chn.raw.Id].raw;
                        if (readstate.LastMessageId != StorageChannel.LastMessageId && !sg.IsMuted)
                            sg.IsUnread = true;
                    }
                ServerList.Items.Add(sg);
            }
        }

        public void RenderDMChannels()
        {
            ClearMessageArea();
            ServerNameButton.Visibility = Visibility.Collapsed;
            FriendsButton.Visibility = Visibility.Visible;

            //Select FriendPanel
            FriendsButton.IsChecked = true;
            friendPanel.Visibility = Visibility.Visible;

            AddChannelButton.Visibility = Visibility.Collapsed;
            ChannelName.Text = "";
            CompChannelName.Text = "";

            ChannelList.Items.Clear();

            foreach (ChannelManager.SimpleChannel channel in ChannelManager.OrderChannels(LocalState.DMs.Values.ToList()))
            {
                ChannelList.Items.Add(channel);
            }
        }

        public void RenderGuildChannels() //App.CurrentGuildId is set
        {
            ClearMessageArea();
            ServerNameButton.Visibility = Visibility.Visible;
            FriendsButton.Visibility = Visibility.Collapsed;
            AddChannelButton.Visibility = Visibility.Visible;
            ChannelName.Text = "";
            CompChannelName.Text = "";
            ServerName.Text = LocalState.Guilds[App.CurrentGuildId].Raw.Name;

            ChannelList.Items.Clear();

            foreach (ChannelManager.SimpleChannel channel in ChannelManager.OrderChannels(LocalState.Guilds[App.CurrentGuildId].channels.Values.ToList()))
            {
                ChannelList.Items.Add(channel);
            }
        }

        public async void RenderMessages() //App.CurrentChannelId is set
        {
            FriendsButton.IsChecked = false;
            friendPanel.Visibility = Visibility.Collapsed;
            PopulateMessageArea();

            ChannelName.Text = (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Type == 0 ? "#" + (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Name : (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Name;
            CompChannelName.Text = ChannelName.Text;

            MessageList.Items.Clear();
            var messages = MessageManager.ConvertMessage((await RESTCalls.GetChannelMessages(App.CurrentChannelId)).ToList());
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    MessageList.Items.Add(message);
                }
            }
        }

        public void UpdateTyping()
        {
            string typingString = "";
            int DisplayedTyperCounter = 0;
            List<string> NamesTyping = new List<string>();
            foreach (var channel in ChannelList.Items)
                (channel as ChannelManager.SimpleChannel).IsTyping = false;
            for (int i = 0; i < LocalState.Typers.Count; i++)
            {
                var typer = LocalState.Typers.ElementAt(i);

                try
                {
                    (ChannelList.Items.FirstOrDefault(
                            x => (x as ChannelManager.SimpleChannel).Id == typer.Key.channelId) as ChannelManager.SimpleChannel)
                        .IsTyping = true;
                }
                catch (Exception exception)
                {
                    App.NavigateToBugReport(exception);
                }
                if (App.CurrentChannelId != null)
                {
                    if (App.CurrentGuildIsDM)
                    {
                        if (App.CurrentChannelId == typer.Key.channelId)
                        {
                            NamesTyping.Add(LocalState.DMs[App.CurrentChannelId].Users.FirstOrDefault(m => m.Id == typer.Key.userId).Username);
                        }
                    }
                    else
                    {
                        if (App.CurrentChannelId == typer.Key.channelId &&
                            LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(typer.Key.userId))
                        {
                            var member = LocalState.Guilds[App.CurrentGuildId].members[typer.Key.userId];
                            string DisplayedName = member.User.Username;
                            if (member.Nick != null) DisplayedName = member.Nick;
                            NamesTyping.Add(DisplayedName);
                        }

                        //TODO: Member list
                        //TODO: Display typing indicator on member list
                    }
                }
            }

            DisplayedTyperCounter = NamesTyping.Count();
            for (int i = 0; i < DisplayedTyperCounter; i++)
            {
                if (i == 0)
                    typingString += NamesTyping.ElementAt(i); //first element, no prefix
                else if (i == 2 && i == DisplayedTyperCounter)
                    typingString += " " + App.GetString("/Main/TypingAnd") + " " + " " + NamesTyping.ElementAt(i); //last element out of 2, prefix = "and"
                else if (i == DisplayedTyperCounter)
                    typingString +=
                        ", " + App.GetString("/Main/TypingAnd") + " " +
                        NamesTyping.ElementAt(i); //last element out of 2, prefix = "and" WITH OXFORD COMMA
                else
                    typingString += ", " + NamesTyping.ElementAt(i); //intermediary element, prefix = comma
            }
            if (DisplayedTyperCounter > 1)
                typingString += " " + App.GetString("/Main/TypingPlural");
            else
                typingString += " " + App.GetString("/Main/TypingSingular");

            if (DisplayedTyperCounter == 0)
            {
                TypingStackPanel.Fade(0, 200).Start();
            }
            else
            {
                TypingIndicator.Text = typingString;
                TypingStackPanel.Fade(1, 200).Start();
            }
        }

        private async void UpdateGuildAndChannelUnread()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    FriendsNotificationCounter.Text = App.FriendNotifications.ToString();
                    if (FriendsNotificationCounter.Text != "0")
                    {
                        ShowFriendsBadge.Begin();
                    } else
                    {
                        HideFriendsBadge.Begin();
                    }
                    int Fullcount = 0;
                    foreach (GuildManager.SimpleGuild guild in ServerList.Items)
                    {
                        GuildManager.SimpleGuild gclone = guild.Clone();
                        gclone.NotificationCount = 0; //Will Change if true
                        gclone.IsUnread = false; //Will change if true
                        if (gclone.Id == "DMs")
                        {
                            //if (App.FriendNotifications > 0 && Storage.Settings.FriendsNotifyFriendRequest) //TODO:
                            //{
                            //    gclone.NotificationCount += App.FriendNotifications;
                            //}

                            foreach (var chn in LocalState.DMs.Values)
                                if (LocalState.RPC.ContainsKey(chn.Id))
                                {
                                    ReadState readstate = LocalState.RPC[chn.Id];
                                    //if (Storage.Settings.FriendsNotifyDMs) //TODO
                                    if (true)
                                    {
                                        gclone.NotificationCount += readstate.MentionCount;
                                        Fullcount += readstate.MentionCount;
                                    }
                                    var StorageChannel = LocalState.DMs[chn.Id];
                                    if (StorageChannel.LastMessageId != null && readstate.LastMessageId != StorageChannel.LastMessageId)
                                        gclone.IsUnread = true;
                                }
                        }
                        else
                        {
                            foreach (var chn in LocalState.Guilds[gclone.Id].channels.Values)
                                if (LocalState.RPC.ContainsKey(chn.raw.Id))
                                {
                                    ReadState readstate = LocalState.RPC[chn.raw.Id];
                                    gclone.NotificationCount += readstate.MentionCount;
                                    Fullcount += readstate.MentionCount;
                                    var StorageChannel = LocalState.Guilds[gclone.Id].channels[chn.raw.Id];
                                        if (StorageChannel.raw.LastMessageId != null && 
                                        readstate.LastMessageId != StorageChannel.raw.LastMessageId && 
                                        LocalState.GuildSettings.ContainsKey(chn.raw.GuildId) ? (LocalState.GuildSettings[chn.raw.GuildId].channelOverrides.ContainsKey(chn.raw.Id) 
                                        ? LocalState.GuildSettings[chn.raw.GuildId].channelOverrides[chn.raw.Id].Muted : false) : false)
                                            gclone.IsUnread = true;
                                }
                        }
                        guild.Id = gclone.Id;
                        guild.ImageURL = gclone.ImageURL;
                        guild.IsDM = gclone.IsDM;
                        guild.IsMuted = gclone.IsMuted;
                        guild.IsUnread = gclone.IsUnread;
                        guild.Name = gclone.Name;
                        guild.NotificationCount = gclone.NotificationCount;
                    }
                    if (App.CurrentGuildIsDM)
                    {
                        foreach (ChannelManager.SimpleChannel sc in ChannelList.Items)
                            if (LocalState.RPC.ContainsKey(sc.Id))
                            {
                                ReadState readstate = LocalState.RPC[sc.Id];
                                sc.NotificationCount = readstate.MentionCount;
                                var StorageChannel = LocalState.DMs[sc.Id];
                                if (StorageChannel.LastMessageId != null &&
                                    readstate.LastMessageId != StorageChannel.LastMessageId)
                                    sc.IsUnread = true;
                                else
                                    sc.IsUnread = false;
                            }
                    }
                    else
                    {
                        if (App.CurrentGuildId != null) //Incase called before intiialization
                        {
                            foreach (ChannelManager.SimpleChannel sc in ChannelList.Items)
                                if (LocalState.RPC.ContainsKey(sc.Id))
                                {
                                    ReadState readstate = LocalState.RPC[sc.Id];
                                    sc.NotificationCount = readstate.MentionCount;
                                    var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                                    if (StorageChannel != null && StorageChannel.raw.LastMessageId != null &&
                                        readstate.LastMessageId != StorageChannel.raw.LastMessageId)
                                        sc.IsUnread = true;
                                    else
                                        sc.IsUnread = false;
                                }
                        }
                    }

                    //if (Storage.Settings.FriendsNotifyFriendRequest) //TODO
                    //{
                    //    Fullcount += App.FriendNotifications;
                    //}

                    //if (App.FriendNotifications > 0) //TODO
                    //{
                    //    FriendsNotificationCounter.Text = App.FriendNotifications.ToString();
                    //    ShowFriendsBadge.Begin();
                    //}
                    //else
                    //{
                    //    HideFriendsBadge.Begin();
                    //}

                    //if (Fullcount > 0) //TODO
                    //{
                    //    ShowBadge.Begin();
                    //    BurgerNotificationCounter.Text = Fullcount.ToString();
                    //}

                });
        }
        #endregion

        private async void App_ReadyRecievedHandler(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     RenderCurrentUser();
                     RenderGuilds();
                     ServerList.SelectedIndex = 0;
                     friendPanel.Load();
                     App.UpdateUnreadIndicators();
                     Loading.Hide(true);
                 });
        }

        private async void App_TypingHandler(object sender, App.TypingArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    UpdateTyping();
                });
        }

        private async void App_UpdateUnreadIndicatorsHandler(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     UpdateGuildAndChannelUnread();
                 });
        }

        #region Messages
        private async void App_MessageCreatedHandler(object sender, App.MessageCreatedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (MessageList.Items.Count > 0)
                     {
                         MessageList.Items.Add(MessageManager.MakeMessage(e.Message));
                     }
                 });
        }

        private async void App_MessageDeletedHandler(object sender, App.MessageDeletedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (MessageList.Items.Count > 0)
                     {
                         foreach (MessageManager.MessageContainer message in MessageList.Items)
                         {
                             if (message.Message.HasValue && message.Message.Value.Id == e.MessageId)
                             {
                                 MessageList.Items.Remove(message);
                             }
                         }
                     }
                 });
        }

        private async void App_MessageEditedHandler(object sender, App.MessageEditedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (MessageList.Items.Count > 0)
                     {
                         foreach (MessageManager.MessageContainer message in MessageList.Items)
                         {
                             if (message.Message.HasValue && message.Message.Value.Id == e.Message.Id)
                             {
                                 message.Message = e.Message;
                             }
                         }
                     }
                 });
        }

        #endregion

        #region Channel
        private async void App_GuildChannelCreatedHandler(object sender, App.GuildChannelCreatedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (ChannelList.Items.Count > 0)
                     {
                         ChannelList.Items.Add(ChannelManager.MakeChannel(LocalState.Guilds[App.CurrentGuildId].channels[e.Channel.Id]));
                     }
                 });
        }

        private async void App_GuildChannelDeletedHandler(object sender, App.GuildChannelDeletedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {

                     if (ChannelList.Items.Count > 0)
                     {
                         foreach (ChannelManager.SimpleChannel channel in ChannelList.Items)
                         {
                             if (channel.Id == e.ChannelId)
                             {
                                 ChannelList.Items.Remove(channel);
                             }
                         }
                     }
                 });
        }
        #endregion

        #region Guilds
        private async void App_GuildCreatedHandler(object sender, App.GuildCreatedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     ServerList.Items.Insert(1, GuildManager.CreateGuild(e.Guild));
                 });
        }
        #endregion
        #endregion

        #endregion

        #region UIEvents
        private void ToggleSplitView(object sender, RoutedEventArgs e)
        {
            if (ServersnChannelsPane.DisplayMode != SplitViewDisplayMode.CompactInline)
            {
                DarkenMessageArea.Begin();
            }
            ServersnChannelsPane.IsPaneOpen = !ServersnChannelsPane.IsPaneOpen;
        }

        private void ServersnChannelsPane_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            if (ServersnChannelsPane.DisplayMode != SplitViewDisplayMode.CompactInline)
            {
                LightenMessageArea.Begin();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((sender as Page).ActualWidth > Storage.Settings.RespUiXl)
            {
                VisualStateManager.GoToState(this, "ExtraLarge", true);
            } else if ((sender as Page).ActualWidth > Storage.Settings.RespUiL)
            {
                VisualStateManager.GoToState(this, "Large", true);
            } else if ((sender as Page).ActualWidth > Storage.Settings.RespUiM)
            {
                VisualStateManager.GoToState(this, "Medium", true);
            } else
            {
                VisualStateManager.GoToState(this, "Small", true);
            }
        }

        private void UserStatus_Checked(object sender, RoutedEventArgs e)
        {
            //TODO: Update status
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            App.NavigateToSettings();
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.NavigateToGuild((ServerList.SelectedItem as GuildManager.SimpleGuild).Id);
        }

        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelList.SelectedItem != null) //Called on clear
            {
                if (App.CurrentGuildIsDM)
                {
                    App.NavigateToDMChannel((ChannelList.SelectedItem as ChannelManager.SimpleChannel).Id, null);
                }
                else
                {
                    App.NavigateToGuildChannel(App.CurrentGuildId, (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Id);
                }
            }
        }

        private void AddChannelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.NavigateToCreateChannel();
        }

        private void AddServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToAddServer();
        }

        private void OpenFriendPanel(object sender, RoutedEventArgs e)
        {
            ClearMessageArea();
            FriendsButton.IsChecked = true;
            ChannelList.SelectedIndex = -1;
            friendPanel.Visibility = Visibility.Visible;
        }

        private void ServerNameButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuildEdit(App.CurrentGuildId);
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            App.CreateMessage(App.CurrentChannelId, MessageBox1.Text);
            MessageBox1.Text = "";
        }

        private void TypingStarted(object sender, TextChangedEventArgs e)
        {
            App.StartTyping(App.CurrentChannelId);
        }

        private void MessageBox1_OpenAdvanced(object sender, RoutedEventArgs e)
        {
            App.NavigateToMessageEditor(MessageBox1.Text);
            MessageBox1.Text = "";
        }

        private void ToggleMemberPane(object sender, RoutedEventArgs e)
        {
            if (MembersPane.DisplayMode != SplitViewDisplayMode.Inline)
            {
                DarkenMessageArea.Begin();
            }
            MembersPane.IsPaneOpen = !MembersPane.IsPaneOpen;
        }

        private void MembersPane_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            if (MembersPane.DisplayMode != SplitViewDisplayMode.Inline)
            {
                LightenMessageArea.Begin();
            }
        }

        private void NavToAbout(object sender, RoutedEventArgs e)
        {
            App.NavigateToAbout();
        }

        private void NavToIAPs(object sender, RoutedEventArgs e)
        {
            App.NavigateToAbout();
        }
        #endregion
    }
}
