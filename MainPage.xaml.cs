using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel.Background;
using Windows.Media.SpeechSynthesis;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Composition;
using Windows.UI;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;
using Windows.Foundation.Metadata;

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
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            sideDrawer.SetupInteraction(ChannelHeader);
            Setup();
            base.OnNavigatedTo(e);
        }
        ScrollViewer MessageScrollviewer;
        ItemsStackPanel messageStacker;
        BackgroundAccessStatus bgAccess;
        static ApplicationTrigger bgTrigger = null;

        public void Setup()
        {
            //Setup UI
            MediumTrigger.MinWindowWidth = Storage.Settings.RespUiM;
            LargeTrigger.MinWindowWidth = Storage.Settings.RespUiL;
            ExtraLargeTrigger.MinWindowWidth = Storage.Settings.RespUiXl;
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();
            var info = new DrillInNavigationTransitionInfo();
            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            SubFrame.ContentTransitions = collection;

            //Setup cinematic mode
            if (App.CinematicMode)
            {
                cmdBar.Visibility = Visibility.Collapsed;
                TitleBarHolder.Visibility = Visibility.Collapsed;
                userButton.Padding = new Thickness(0, 0, 0, 48);
                userButton.Height = 112;
                ServerList.Padding = new Thickness(0, 84, 0, 48);
                ChannelList.Padding = new Thickness(0, 84, 0, 48);
                CinematicChannelName.Visibility = Visibility.Visible;
                CinematicGuildName.Visibility = Visibility.Visible;
                friendPanel.Margin = new Thickness(0, 84, 0, 0);
                MessageList.Padding = new Thickness(0, 84, 0, 0);
                MessageArea.Margin = new Thickness(0);
                CinematicMask1.Visibility = Visibility.Visible;
                CinematicMask2.Visibility = Visibility.Visible;
                ControllerHints.Visibility = Visibility.Visible;
                
            }

            //Setup BackButton
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            //Setup Controller input
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            //Setup MessageList infinite scroll
            MessageScrollviewer = Common.GetScrollViewer(MessageList);
            if (MessageScrollviewer != null)
            {
                MessageScrollviewer.ViewChanged += MessageScrollviewer_ViewChanged;
            }



            //Hook up the login Event
            App.LoggingInHandler += App_LoggingInHandlerAsync;
            //Verify if a token exists, if not navigate to login page
            if(App.LoggedIn() == false)
            {
                SubFrameNavigator(typeof(LogScreen));
                return;
            }
            else
            {
                App_LoggingInHandlerAsync(null,null);
            }
        }

        Stack<Tuple<string, string>> navigationHistory = new Stack<Tuple<string, string>>();
        Tuple<string, string> currentPage = new Tuple<string, string>(null, null);

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (SubFrame.Visibility == Visibility.Visible)
            {
                App.SubpageClose();
            } else
            {
                if (navigationHistory.Count > 0)
                {
                    var page = navigationHistory.Pop();
                    if (page.Item1 != null)
                    {
                        App.NavigateToGuildChannel(page.Item1, page.Item2, null, false, true);
                    }
                    else
                    {
                        if (page.Item2 != null)
                        {
                            App.NavigateToDMChannel(page.Item2, null, false, true);
                        } else
                        {
                            App.NavigateToDMChannel(null, null, false, true);
                        }
                    }
                } else
                {
                    e.Handled = true;
                }
            }
        }

        bool DisableLoadingMessages;
        private void MessageScrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (MessageList.Items.Count > 0)
            {
                double fromTop = MessageScrollviewer.VerticalOffset;
                double fromBottom = MessageScrollviewer.ScrollableHeight - fromTop;
                if (fromTop < 100 && !DisableLoadingMessages)
                    LoadOlderMessages();
                if (fromBottom < 100 && !DisableLoadingMessages)
                    LoadNewerMessages();
            }
        }

        public void SetupEvents()
        {
            //LogOut
            App.LogOutHandler += App_LogOutHandler;
            //Navigation
            App.NavigateToGuildHandler += App_NavigateToGuildHandler;
            App.NavigateToGuildChannelHandler += App_NavigateToGuildChannelHandler;
            App.NavigateToDMChannelHandler += App_NavigateToDMChannelHandler;
            //SubPages
            App.SubpageClosedHandler += App_SubpageClosedHandler;
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
            App.NavigateToIAPSHandler += App_NavigateToIAPSHandler;
            //Flyouts
            App.MenuHandler += App_MenuHandler;
            App.MentionHandler += App_MentionHandler;
            App.ShowMemberFlyoutHandler += App_ShowMemberFlyoutHandler;
            //Link
            App.LinkClicked += App_LinkClicked;
            //API
            App.CreateMessageHandler += App_CreateMessageHandler;
            typingCooldown.Tick += TypingCooldown_Tick;
            App.StartTypingHandler += App_StartTypingHandler;
            App.AddFriendHandler += App_AddFriendHandler;
            App.BlockUserHandler += App_BlockUserHandler;
            App.MarkMessageAsReadHandler += App_MarkMessageAsReadHandler;
            App.MarkChannelAsReadHandler += App_MarkChannelAsReadHandler;
            App.MarkGuildAsReadHandler += App_MarkGuildAsReadHandler;
            App.MuteChannelHandler += App_MuteChannelHandler;
            App.MuteGuildHandler += App_MuteGuildHandler;
            App.RemoveFriendHandler += App_RemoveFriendHandler;
            App.UpdatePresenceHandler += App_UpdatePresenceHandler;
            App.VoiceConnectHandler += App_VoiceConnectHandler;
            //UpdateUI
            App.ReadyRecievedHandler += App_ReadyRecievedHandler;
            App.TypingHandler += App_TypingHandler;
            App.UpdateUnreadIndicatorsHandler += App_UpdateUnreadIndicatorsHandler;
            App.UserStatusChangedHandler += App_UserStatusChangedHandler; ;
            //UpdateUI-Messages
            App.MessageCreatedHandler += App_MessageCreatedHandler;
            App.MessageDeletedHandler += App_MessageDeletedHandler;
            App.MessageEditedHandler += App_MessageEditedHandler;
            //UpdateUI-Channels
            App.GuildChannelCreatedHandler += App_GuildChannelCreatedHandler;
            //UpdateUI-Guilds
            App.GuildCreatedHandler += App_GuildCreatedHandler;
            App.GuildChannelDeletedHandler += App_GuildChannelDeletedHandler;
            //UpdateUI-Members
            App.MembersUpdatedHandler += App_MembersUpdatedHandler;

            //Auto selects
            App.SelectGuildChannelHandler += App_SelectGuildChannelHandler;
            
        }

        private void App_MentionHandler(object sender, App.MentionArgs e)
        {
            if (MessageBox1.Text.Trim() == "")
                MessageBox1.Text = "@" + e.Username + "#" + e.Discriminator;
            else
                MessageBox1.Text = MessageBox1.Text + " @" + e.Username + "#" + e.Discriminator;
        }

        private void App_SelectGuildChannelHandler(object sender, App.GuildChannelSelectArgs e)
        {
            string guild = e.GuildId;
            string channel = e.ChannelId;
            foreach(GuildManager.SimpleGuild g in ServerList.Items)
            {
                if (g.Id == guild)
                    ServerList.SelectedItem = g;
            }
            if(channel != null)
            {
                foreach(ChannelManager.SimpleChannel c in ChannelList.Items)
                {
                    if (c.Id == e.ChannelId)
                        ChannelList.SelectedItem = c;
                }
            }
        }

        public void ClearData()
        {
            LocalState.CurrentUser = new SharedModels.User();
            LocalState.DMs.Clear();
            LocalState.Friends.Clear();
            LocalState.Guilds.Clear();
            LocalState.GuildSettings.Clear();
            LocalState.Notes.Clear();
            LocalState.PresenceDict.Clear();
            LocalState.RPC.Clear();
            LocalState.Typers.Clear();
            LocalState.TyperTimers.Clear();
            LocalState.VoiceDict.Clear();

            //LogOut
            App.LogOutHandler -= App_LogOutHandler;
            //Navigation
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
            App.NavigateToGuildChannelHandler -= App_NavigateToGuildChannelHandler;
            App.NavigateToDMChannelHandler -= App_NavigateToDMChannelHandler;
            //SubPages
            App.SubpageClosedHandler -= App_SubpageClosedHandler; ;
            App.NavigateToBugReportHandler -= App_NavigateToBugReportHandler;
            App.NavigateToChannelEditHandler -= App_NavigateToChannelEditHandler;
            App.NavigateToCreateBanHandler -= App_NavigateToCreateBanHandler;
            App.NavigateToCreateServerHandler -= App_NavigateToCreateServerHandler;
            App.NavigateToDeleteChannelHandler -= App_NavigateToDeleteChannelHandler;
            App.NavigateToDeleteServerHandler -= App_NavigateToDeleteServerHandler;
            App.NavigateToGuildEditHandler -= App_NavigateToGuildEditHandler;
            App.NavigateToJoinServerHandler -= App_NavigateToJoinServerHandler;
            App.NavigateToLeaveServerHandler -= App_NavigateToLeaveServerHandler;
            App.NavigateToNicknameEditHandler -= App_NavigateToNicknameEditHandler;
            App.NavigateToProfileHandler -= App_NavigateToProfileHandler;
            App.OpenAttachementHandler -= App_OpenAttachementHandler;
            App.NavigateToChannelTopicHandler -= App_NavigateToChannelTopicHandler;
            App.NavigateToCreateChannelHandler -= App_NavigateToCreateChannelHandler;
            App.NavigateToSettingsHandler -= App_NavigateToSettingsHandler;
            App.NavigateToAboutHandler -= App_NavigateToAboutHandler;
            App.NavigateToAddServerHandler -= App_NavigateToAddServerHandler;
            App.NavigateToMessageEditorHandler -= App_NavigateToMessageEditorHandler;
            App.NavigateToIAPSHandler -= App_NavigateToIAPSHandler;
            //Flyouts
            App.MenuHandler -= App_MenuHandler;
            App.MentionHandler -= App_MentionHandler;
            App.ShowMemberFlyoutHandler -= App_ShowMemberFlyoutHandler;
            //Link
            App.LinkClicked -= App_LinkClicked;
            //API
            App.CreateMessageHandler -= App_CreateMessageHandler;
            App.StartTypingHandler -= App_StartTypingHandler;
            App.AddFriendHandler -= App_AddFriendHandler;
            App.BlockUserHandler -= App_BlockUserHandler;
            App.MarkChannelAsReadHandler -= App_MarkChannelAsReadHandler;
            App.MarkGuildAsReadHandler -= App_MarkGuildAsReadHandler;
            App.MuteChannelHandler -= App_MuteChannelHandler;
            App.MuteGuildHandler -= App_MuteGuildHandler;
            App.RemoveFriendHandler -= App_RemoveFriendHandler;
            App.UpdatePresenceHandler -= App_UpdatePresenceHandler;
            //UpdateUI
            App.ReadyRecievedHandler -= App_ReadyRecievedHandler;
            App.TypingHandler -= App_TypingHandler;
            App.UpdateUnreadIndicatorsHandler -= App_UpdateUnreadIndicatorsHandler;
            App.UserStatusChangedHandler -= App_UserStatusChangedHandler; ;
            //UpdateUI-Messages
            App.MessageCreatedHandler -= App_MessageCreatedHandler;
            App.MessageDeletedHandler -= App_MessageDeletedHandler;
            App.MessageEditedHandler -= App_MessageEditedHandler;
            //UpdateUI-Channels
            App.GuildChannelCreatedHandler -= App_GuildChannelCreatedHandler;
            //UpdateUI-Guilds
            App.GuildCreatedHandler -= App_GuildCreatedHandler;
            App.GuildChannelDeletedHandler -= App_GuildChannelDeletedHandler;

        }

        public static bool RegisterBacgkround()
        {
            try
            {
                var task = new BackgroundTaskBuilder
                {
                    Name = "Discord UWP Notifier",
                    TaskEntryPoint = typeof(DiscordBackgroundTask1.MainClass).ToString()
                };

                bgTrigger = new ApplicationTrigger();
                task.SetTrigger(bgTrigger);

                task.Register();
                Console.WriteLine("Task registered");
                return true;
            }
            catch /*(Exception ex)*/
            {
                return false;
            }
        }

        #region AppEvents

        #region LogIn
    private async void App_LoggingInHandlerAsync(object sender, EventArgs e)
        {
            Loading.Show(false);
            SubFrameMask.Opacity = 0;
            await RESTCalls.SetupToken();
            if (App.IsMobile)
            {
                TitleBarHolder.Visibility = Visibility.Collapsed;
            }
            if (App.LoggedIn() && (App.GatewayCreated))
            {
                SetupEvents();
                GatewayManager.StartGateway();

                try
                {
                    bgAccess = await BackgroundExecutionManager.RequestAccessAsync();
                    switch (bgAccess)
                    {
                        case BackgroundAccessStatus.Unspecified:
                            Console.WriteLine("Unspecified result");
                            break;
                        case BackgroundAccessStatus.AlwaysAllowed:
                            if (RegisterBacgkround() == true)
                            {
                                var result = await bgTrigger.RequestAsync();
                                Console.WriteLine(result.ToString());
                            }
                            break;
                        case BackgroundAccessStatus.AllowedSubjectToSystemPolicy:
                            if (RegisterBacgkround() == true)
                            {
                                var result = await bgTrigger.RequestAsync();
                                Console.WriteLine(result.ToString());
                            }
                            break;
                        case BackgroundAccessStatus.DeniedBySystemPolicy:
                            Console.WriteLine("Denied by system policy");
                            break;
                        case BackgroundAccessStatus.DeniedByUser:
                            Console.WriteLine("Denied by user");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            SubFrame.Visibility = Visibility.Collapsed;
            SetupEvents();
            GatewayManager.StartGateway();
        }
        #endregion

        #region LogOut
        private void App_LogOutHandler(object sender, EventArgs e)
        {
            var creds = Storage.PasswordVault.Retrieve("Token", LocalState.CurrentUser.Email);
            Storage.PasswordVault.Remove(creds);

            ClearData();

            SubFrameNavigator(typeof(LogScreen));
        }
        #endregion

        #region Navigation
        private void App_NavigateToGuildHandler(object sender, App.GuildNavigationArgs e)
        {
            App.CurrentGuildIsDM = e.GuildId == "DMs"; //Could combine...
            if (e.GuildId != "DMs")
            {
                MemberToggle.Visibility = Visibility.Visible;
                
                foreach (GuildManager.SimpleGuild guild in ServerList.Items)
                {
                    if (guild.Id == e.GuildId)
                    {
                        ServerList.SelectedItem = guild;
                    }
                }

                App.CurrentGuildId = e.GuildId;
                RenderMembers();
                RenderGuildChannels();
            } else
            {
                App.CurrentGuildId = null;
                MemberToggle.Visibility = Visibility.Collapsed;
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
            App.UpdateUnreadIndicators();
        }
        private void App_NavigateToGuildChannelHandler(object sender, App.GuildChannelNavigationArgs e)
        {
            if (App.CurrentGuildId == e.GuildId)
            {
                if (!e.OnBack)
                {
                    navigationHistory.Push(currentPage);
                }

                App.CurrentChannelId = e.ChannelId;
                RenderMessages();
                App.MarkChannelAsRead(e.ChannelId);
                currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);

                if (e.OnBack)
                {
                    foreach (ChannelManager.SimpleChannel chn in ChannelList.Items)
                    {
                        if (chn.Id == e.ChannelId)
                        {
                            lastChangeProgrammatic = true;
                            ChannelList.SelectedItem = chn;
                        }
                    }
                }
            }
            else //Out of guild navigation
            {
                if (!e.OnBack)
                {
                    navigationHistory.Push(currentPage);
                }

                foreach (GuildManager.SimpleGuild guild in ServerList.Items)
                {
                    if (guild.Id == e.GuildId)
                    {
                        ServerList.SelectedItem = guild;
                    }
                }
                foreach (ChannelManager.SimpleChannel chn in ChannelList.Items)
                {
                    if (chn.Id == e.ChannelId)
                    {
                        lastChangeProgrammatic = true;
                        ChannelList.SelectedItem = chn;
                    }
                }

                App.CurrentChannelId = e.ChannelId;
                RenderMessages();
                App.MarkChannelAsRead(e.ChannelId);
                currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);
            }
        }
        private void App_NavigateToDMChannelHandler(object sender, App.DMChannelNavigationArgs e)
        {
            if (e.ChannelId != null) //Nav by ChannelId
            {
                if (!e.OnBack)
                {
                    navigationHistory.Push(currentPage);
                }

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

                App.MarkChannelAsRead(e.ChannelId);
                currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);

                if (e.OnBack)
                {
                    foreach (ChannelManager.SimpleChannel chn in ChannelList.Items)
                    {
                        if (chn.Id == e.ChannelId)
                        {
                            lastChangeProgrammatic = true;
                            ChannelList.SelectedItem = chn;
                        }
                    }
                }

            } else if (e.UserId != null) //Nav by UserId
            {
                if (!e.OnBack)
                {
                    navigationHistory.Push(currentPage);
                }
                //TODO: Nav by UserId
            } else //Nav to Friends
            {
                if (App.CurrentGuildIsDM)
                {
                    OpenFriendPanel(null, null);
                } else
                {
                    ServerList.SelectedIndex = 0;
                }
            }
        }

        #endregion

        #region SubPages
        private void SubFrameNavigator(Type page, object args = null)
        {
            if (Storage.Settings.ExpensiveRender)
            {
                content.Blur(2, 300).Start();
            }
            SubFrameMask.Fade(0.6f, 500, 0, 0).Start();
            SubFrame.Visibility = Visibility.Visible;
            SubFrame.Navigate(page, args);
        }
        private void App_SubpageClosedHandler(object sender, EventArgs e)
        {
            if (Storage.Settings.ExpensiveRender)
            {
                content.Blur(0, 600).Start();
            }
            else
            {
                content.Blur(0, 0).Start();
            }
            SubFrameMask.Fade(0f, 300, 0, 0).Start();
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
            if(e.User.Bot)
                SubFrameNavigator(typeof(SubPages.UserProfile), e.User);
            else
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
        private void App_NavigateToAboutHandler(object sender, bool e)
        {
            if (!e)
            {
                SubFrameNavigator(typeof(SubPages.About));
            } else
            {
                SubFrameNavigator(typeof(SubPages.WhatsNew));
            }
        }
        private void App_NavigateToAddServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.AddServer));
        }
        private void App_NavigateToMessageEditorHandler(object sender, App.MessageEditorNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.ExtendedMessageEditor), e.Content);
        }
        private void App_NavigateToIAPSHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.InAppPurchases));
        }
        #endregion

        #region Flyouts
        private void App_MenuHandler(object sender, App.MenuArgs e)
        {
            e.Flyout.ShowAt((sender as UIElement), e.Point);
        }

        private void App_ShowMemberFlyoutHandler(object sender, App.ProfileNavigationArgs e)
        {
            if (!App.CurrentGuildIsDM)
            {
                var member = LocalState.Guilds[App.CurrentGuildId].members[e.User.Id];
                FlyoutManager.MakeUserDetailsFlyout(member).ShowAt(sender as FrameworkElement);
            }
            else
            {
                FlyoutManager.MakeUserDetailsFlyout(e.User).ShowAt(sender as FrameworkElement);
            }
        }
        #endregion

        #region Link
        private async void App_LinkClicked(object sender, MarkdownTextBlock.LinkClickedEventArgs e)
        {
            if (e.Link.StartsWith("#"))
            {
                string val = e.Link.Remove(0, 1);
                foreach (ChannelManager.SimpleChannel item in ChannelList.Items)
                {
                    if (item.Id == val)
                    {
                        ChannelList.SelectedItem = item;
                        ChannelList_ItemClick(null, null);
                    }
                }
            }
            else if (e.Link.StartsWith("@!"))
            {
                string val = e.Link.Remove(0, 2);
                App.NavigateToProfile(LocalState.Guilds[App.CurrentGuildId].members[val].User);
            }
            else if (e.Link.StartsWith("@&"))
            {
                string val = e.Link.Remove(0, 2);
                //TODO Fix this shit
                MembersListView.ScrollIntoView(memberscvs.FirstOrDefault(x => x.Value.MemberDisplayedRole.Id == val));
                sideDrawer.OpenRight();
            }
            else if (e.Link.StartsWith("@"))
            {
                string val = e.Link.Remove(0, 1);
                App.NavigateToProfile(LocalState.Guilds[App.CurrentGuildId].members[val].User);
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }
        #endregion

        #region API
        private async void App_CreateMessageHandler(object sender, App.CreateMessageArgs e)
        {
            MessageList.Items.Add(MessageManager.MakeMessage(e.ChannelId, e.Message));
            await RESTCalls.CreateMessage(e.ChannelId, e.Message);
        }

        //The typing cooldown disables the trigger typing event from being fired if it was already triggered less than 5 seconds ago
        //This is to avoid 429 errors (too many requests) because otherwise it would fire on every letter
        //This also improves performance
        DispatcherTimer typingCooldown = new DispatcherTimer() { Interval=TimeSpan.FromSeconds(5) };
        private void TypingCooldown_Tick(object sender, object e)
        {
            typingCooldown.Stop();
        }
        private async void App_StartTypingHandler(object sender, App.StartTypingArgs e)
        {
            if (!typingCooldown.IsEnabled)
            {
                await RESTCalls.TriggerTypingIndicator(e.ChannelId);
                typingCooldown.Start();
            }
            
        }

        private void App_AddFriendHandler(object sender, App.AddFriendArgs e)
        {

        }

        private void App_BlockUserHandler(object sender, App.BlockUserArgs e)
        {

        }

        private async void App_MarkMessageAsReadHandler(object sender, App.MarkMessageAsReadArgs e)
        {
            await RESTCalls.AckMessage(e.ChannelId, e.MessageId);
        }

        private async void App_MarkChannelAsReadHandler(object sender, App.MarkChannelAsReadArgs e)
        {
            //Assumes you marked it from active guild
            if (!App.CurrentGuildIsDM)
            {
                if (LocalState.Guilds[App.CurrentGuildId].channels.ContainsKey(e.ChannelId))
                {
                    await RESTCalls.AckMessage(e.ChannelId, LocalState.Guilds[App.CurrentGuildId].channels[e.ChannelId].raw.LastMessageId);
                    //Update Unread called on Gateway Event
                }
            } else
            {
                await RESTCalls.AckMessage(e.ChannelId, LocalState.DMs[e.ChannelId].LastMessageId);
            }
        }

        private async void App_MarkGuildAsReadHandler(object sender, App.MarkGuildAsReadArgs e)
        {
            await RESTCalls.AckGuild(e.GuildId);
            //Update Unread called on Gateway Event
        }

        private async void App_MuteChannelHandler(object sender, App.MuteChannelArgs e)
        {
            //Assumes you muted it from active guild
            var returned = await RESTCalls.ModifyGuildSettings(App.CurrentGuildId, new SharedModels.GuildSetting() { ChannelOverrides = new List<SharedModels.ChannelOverride> { new ChannelOverride() { Channel_Id = e.ChannelId, Muted = LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.ContainsKey(e.ChannelId) ? !(LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[e.ChannelId].Muted) : true } } });

            foreach (var chn in returned.ChannelOverrides)
            {
                if (chn.Channel_Id == e.ChannelId)
                {
                    LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[e.ChannelId] = chn;
                }
            }
            App.UpdateUnreadIndicators();
        }

        private async void App_MuteGuildHandler(object sender, App.MuteGuildArgs e)
        {
            LocalState.GuildSettings[e.GuildId] = new LocalModels.GuildSetting(await RESTCalls.ModifyGuildSettings(e.GuildId, new SharedModels.GuildSetting() { Muted = !(LocalState.GuildSettings[e.GuildId].raw.Muted) }));
            App.UpdateUnreadIndicators();
        }

        private void App_RemoveFriendHandler(object sender, App.RemoveFriendArgs e)
        {

        }

        private async void App_UpdatePresenceHandler(object sender, App.UpdatePresenceArgs e)
        {
            await RESTCalls.ChangeUserSettings(e.Status);
        }

        private async void App_VoiceConnectHandler(object sender, App.VoiceConnectArgs e)
        {
            await GatewayManager.Gateway.VoiceStatusUpdate(e.GuildId, e.ChannelId, true, false);
        }
        #endregion

        #region UpdateUI

        #region RenderElement
        public void PopulateMessageArea()
        {
            MessageList.Items.Clear();
            PinnedMessageList.Items.Clear();
            SendMessage.Visibility = Visibility.Visible;
            if (Page.ActualWidth <= 500)
            {
                //CompressedChannelHeader.Visibility = Visibility.Visible;
            }
            PinnedMessags.Visibility = Visibility.Visible;
        }
        public void ClearMessageArea()
        {
            friendPanel.Visibility = Visibility.Collapsed;
            MessageList.Items.Clear();
            SendMessage.Visibility = Visibility.Collapsed;
            //CompressedChannelHeader.Visibility = Visibility.Collapsed;
            PinnedMessags.Visibility = Visibility.Collapsed;
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
            ServerList.Items.Clear();
            GuildManager.SimpleGuild DM = new GuildManager.SimpleGuild();
            DM.Id = "DMs";
            DM.Name = App.GetString("/Main/DirectMessages");
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
                sg.IsValid = guild.Value.valid;
                ServerList.Items.Add(sg);
            }
        }

        public void RenderDMChannels()
        {
            ClearMessageArea();

            ChannelLoading.IsActive = true;
            ChannelLoading.Visibility = Visibility.Visible;

            ServerNameButton.Visibility = Visibility.Collapsed;
            FriendsItem.Visibility = Visibility.Visible;
            DirectMessageBlock.Visibility = Visibility.Visible;

            //Select FriendPanel
            FriendsItem.IsSelected = true;
            friendPanel.Visibility = Visibility.Visible;

            AddChannelButton.Visibility = Visibility.Collapsed;
            ChannelName.Text = /*CompChannelName.Text =*/ ChannelTopic.Text = /*CompChannelTopic.Text =*/ "";

            ChannelList.Items.Clear();

            foreach (ChannelManager.SimpleChannel channel in ChannelManager.OrderChannels(LocalState.DMs.Values.ToList()))
            {
                if (App.CurrentGuildIsDM)
                {
                    ChannelList.Items.Add(channel);
                }
            }

            ChannelLoading.IsActive = false;
            ChannelLoading.Visibility = Visibility.Collapsed;
        }

        public void RenderGuildChannels() //App.CurrentGuildId is set
        {
            ClearMessageArea();

            ChannelLoading.IsActive = true;
            ChannelLoading.Visibility = Visibility.Visible;

            ServerNameButton.Visibility = Visibility.Visible;
            FriendsItem.Visibility = Visibility.Collapsed;
            DirectMessageBlock.Visibility = Visibility.Collapsed;
            AddChannelButton.Visibility = Visibility.Collapsed;

            ChannelName.Text = /*CompChannelName.Text =*/ ChannelTopic.Text = /*CompChannelTopic.Text =*/ "";

            ServerName.Text = LocalState.Guilds[App.CurrentGuildId].Raw.Name;

            ChannelList.Items.Clear();

            foreach (ChannelManager.SimpleChannel channel in ChannelManager.OrderChannels(LocalState.Guilds[App.CurrentGuildId].channels.Values.ToList()))
            {
                ChannelList.Items.Add(channel);
            }

            ChannelLoading.IsActive = false;
            ChannelLoading.Visibility = Visibility.Collapsed;
        }

        //bool MessageRange_LastMessage = false;
        public async void RenderMessages() //App.CurrentChannelId is set
        {
            MessagesLoading.Visibility = Visibility.Visible;
            FriendsItem.IsSelected = false;
            friendPanel.Visibility = Visibility.Collapsed;
            PopulateMessageArea();

            if (UISize.CurrentState == Small)
            {
                sideDrawer.CloseLeft();
            }

            ChannelName.Text = (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Type == 0 ? "#" + (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Name : (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Name;
            //CompChannelName.Text = ChannelName.Text;
            ChannelTopic.Text = (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Type == 0 ? LocalState.Guilds[App.CurrentGuildId].channels[(ChannelList.SelectedItem as ChannelManager.SimpleChannel).Id].raw.Topic : "";
            //CompChannelTopic.Text = ChannelTopic.Text;
            if (ChannelTopic.Text == null || ChannelTopic.Text.Trim() == "")
            {
                ChannelTopic.Visibility = Visibility.Collapsed;
                ChannelName.Margin = new Thickness(0,10,0,0);
            }
                
            else
            {
                ChannelTopic.Visibility = Visibility.Visible;
                ChannelName.Margin = new Thickness(0,0,0,0);
            }

            MessageList.Items.Clear();
            IEnumerable<Message> emessages = null;
            await Task.Run(async () =>
            {
                emessages = await RESTCalls.GetChannelMessages(App.CurrentChannelId);
            });
            if (emessages != null)
            {
                var messages = await MessageManager.ConvertMessage(emessages.ToList());
                if (messages != null)
                {
                    foreach (var message in messages)
                    {
                        MessageList.Items.Add(message);
                    }
                }
            } else
            {
                //TODO: Check offline status and potentially set to offline mode
            }
            IEnumerable<Message> epinnedmessages = null;
            await Task.Run(async () =>
            {
                epinnedmessages = await RESTCalls.GetChannelPinnedMessages(App.CurrentChannelId);
            });
            if (epinnedmessages != null)
            {
                var pinnedmessages = await MessageManager.ConvertMessage(epinnedmessages.ToList());
                if (pinnedmessages != null)
                {
                    foreach (var message in pinnedmessages)
                    {
                        PinnedMessageList.Items.Add(message);
                    }
                }
            }
            MessagesLoading.Visibility = Visibility.Collapsed;
            sideDrawer.CloseLeft();
        }

        public async void RenderMembers()
        {
            memberscvs.Clear();
            if (!App.CurrentGuildIsDM && App.CurrentGuildId != null) //Reduntant I know
            {
                //await GatewayManager.Gateway.RequestAllGuildMembers(App.CurrentGuildId);
                var members = await RESTCalls.GetGuildMembers(App.CurrentGuildId);
                if (members == null)
                {
                    //TODO: Check offline status and potentially set to offline mode
                }
                foreach (var member in members)
                {
                    if (!LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(member.User.Id))
                    {
                        LocalState.Guilds[App.CurrentGuildId].members.Add(member.User.Id, member);
                    }
                    else
                    {
                        LocalState.Guilds[App.CurrentGuildId].members[member.User.Id] = member;
                    }
                }
                int totalrolecounter = 0;

                if (LocalState.Guilds[App.CurrentGuildId].Raw.Roles != null)
                {
                    foreach (Role role in LocalState.Guilds[App.CurrentGuildId].Raw.Roles)
                    {
                        Role roleAlt = role;
                        if (role.Hoist)
                        {
                            int rolecounter = 0;
                            foreach (GuildMember m in LocalState.Guilds[App.CurrentGuildId].members.Values)
                                if (m.Roles.FirstOrDefault() == role.Id) rolecounter++;
                            totalrolecounter += rolecounter;
                            roleAlt.MemberCount = rolecounter;
                        }
                        if (LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(role.Id))
                        {
                            LocalState.Guilds[App.CurrentGuildId].roles[role.Id] = roleAlt;
                        }
                        else
                        {
                            LocalState.Guilds[App.CurrentGuildId].roles.Add(role.Id, roleAlt);
                        }
                    }
                    int everyonecounter = LocalState.Guilds[App.CurrentGuildId].members.Count() - totalrolecounter;
                    foreach (GuildMember member in LocalState.Guilds[App.CurrentGuildId].members.Values)
                    {
                        var m = new Member(member);
                        if (m.Raw.Roles.FirstOrDefault() != null &&
                            LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(m.Raw.Roles.FirstOrDefault()) &&
                            LocalState.Guilds[App.CurrentGuildId].roles[m.Raw.Roles.FirstOrDefault()].Hoist)
                        {
                            m.MemberDisplayedRole = MemberManager.GetRole(m.Raw.Roles.FirstOrDefault(), App.CurrentGuildId, everyonecounter);
                        }
                        else
                        {
                            m.MemberDisplayedRole = MemberManager.GetRole(null, App.CurrentGuildId, everyonecounter);
                        }
                        if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                        {
                            m.status = LocalState.PresenceDict[m.Raw.User.Id];
                        }
                        else
                        {
                            m.status = new Presence() { Status = "offline", Game = null };
                        }
                        if (memberscvs.ContainsKey(m.Raw.User.Id))
                        {
                            memberscvs.Remove(m.Raw.User.Id);
                        }
                        memberscvs.Add(m.Raw.User.Id, m);
                    }
                    try
                    {
                        var sortedMembers =
                            memberscvs.GroupBy(m => m.Value.MemberDisplayedRole).OrderByDescending(x => x.Key.Position);

                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () =>
                            {
                            // MembersCVS = new CollectionViewSource();
                            MembersCvs.Source = sortedMembers;
                            });
                    }
                    catch
                    {

                    }

                    //else
                    //    MembersCVS.Source = memberscvs.SkipWhile(m => m.Value.status.Status == "offline").GroupBy(m => m.Value.MemberDisplayedRole).OrderBy(m => m.Key.Position).ToList();
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
                catch /*(Exception exception)*/
                {
                    //App.NavigateToBugReport(exception);
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
                    FriendNotificationCounter.Text = App.FriendNotifications.ToString();
                    if (FriendNotificationCounter.Text != "0")
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
                            if (App.FriendNotifications > 0 && Storage.Settings.FriendsNotifyFriendRequest)
                            {
                                gclone.NotificationCount += App.FriendNotifications;
                            }

                            foreach (var chn in LocalState.DMs.Values)
                                if (LocalState.RPC.ContainsKey(chn.Id))
                                {
                                    ReadState readstate = LocalState.RPC[chn.Id];
                                    if (Storage.Settings.FriendsNotifyDMs)
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
                            if (LocalState.GuildSettings.ContainsKey(gclone.Id))
                            {
                                gclone.IsMuted = LocalState.GuildSettings[gclone.Id].raw.Muted;
                            } else
                            {
                                gclone.IsMuted = false;
                            }
                            foreach (var chn in LocalState.Guilds[gclone.Id].channels.Values)
                                if (LocalState.RPC.ContainsKey(chn.raw.Id))
                                {
                                    var chan = LocalState.Guilds[gclone.Id].channels[chn.raw.Id];
                                    ReadState readstate = LocalState.RPC[chn.raw.Id];

                                    bool Muted = LocalState.GuildSettings.ContainsKey(gclone.Id) ? (LocalState.GuildSettings[gclone.Id].channelOverrides.ContainsKey(chan.raw.Id) ?
                                    LocalState.GuildSettings[gclone.Id].channelOverrides[chan.raw.Id].Muted
                                    : false) :
                                    false;

                                    gclone.NotificationCount += readstate.MentionCount;
                                    Fullcount += readstate.MentionCount;

                                    if (chan.raw.LastMessageId != null
                                    && chan.raw.LastMessageId != readstate.LastMessageId && (Storage.Settings.mutedChnEffectServer || !Muted)
                                    ) //if channel is unread and not muted
                                           //if(chan.raw.LastMessageId != null && chan.raw.LastMessageId != readstate.LastMessageId)
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
                                if (LocalState.RPC.ContainsKey(sc.Id) && LocalState.Guilds[App.CurrentGuildId].channels.ContainsKey(sc.Id))
                                {
                                    ReadState readstate = LocalState.RPC[sc.Id];
                                    sc.NotificationCount = readstate.MentionCount;
                                    var StorageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                                    if (StorageChannel != null && StorageChannel.raw.LastMessageId != null &&
                                        readstate.LastMessageId != StorageChannel.raw.LastMessageId)
                                        sc.IsUnread = true;
                                    else
                                        sc.IsUnread = false;

                                    sc.IsMuted = LocalState.GuildSettings.ContainsKey(App.CurrentGuildId) && LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.ContainsKey(sc.Id) && LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[sc.Id].Muted;
                                }
                        }
                    }

                    if (Storage.Settings.FriendsNotifyFriendRequest)
                    {
                        Fullcount += App.FriendNotifications;
                    }

                    if (App.FriendNotifications > 0)
                    {
                        FriendNotificationCounter.Text = App.FriendNotifications.ToString();
                        ShowFriendsBadge.Begin();
                    }
                    else
                    {
                        HideFriendsBadge.Begin();
                    }

                    //if (Fullcount > 0) //TODO
                    //{
                    //ShowBadge.Begin();
                    //BurgerNotificationCounter.Text = Fullcount.ToString();
                    //}

                });
        }

        private async void LoadOlderMessages()
        {
            DisableLoadingMessages = true;
            var messages = await MessageManager.ConvertMessage((await RESTCalls.GetChannelMessagesBefore(App.CurrentChannelId, (MessageList.Items.FirstOrDefault(x => (x as MessageManager.MessageContainer).Message.HasValue) as MessageManager.MessageContainer).Message.Value.Id)).ToList());
            if (messages != null)
            {
                messages.Reverse();
                foreach (var message in messages)
                {
                    MessageList.Items.Insert(0, message);
                    if(MessageList.Items.Count > 150)
                        MessageList.Items.RemoveAt(MessageList.Items.Count - 1);
                }
            }
         //   await Task.Delay(1500);
            DisableLoadingMessages = false;
        }
        private async void LoadNewerMessages()
        {
            var offset = MessageScrollviewer.VerticalOffset;
            DisableLoadingMessages = true;
            var messages = await MessageManager.ConvertMessage((await RESTCalls.GetChannelMessagesAfter(App.CurrentChannelId, (MessageList.Items.LastOrDefault(x => (x as MessageManager.MessageContainer).Message.HasValue) as MessageManager.MessageContainer).Message.Value.Id)).ToList());
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    MessageList.Items.Add(message);
                    if(MessageList.Items.Count > 150)
                        MessageList.Items.RemoveAt(0);
                }
                MessageScrollviewer.ChangeView(0, offset, 1);
            }
         //   await Task.Delay(1500);
            DisableLoadingMessages = false;
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
                     App.FullyLoaded = true;
                     if (App.PostLoadTask != null)
                     {
                         switch (App.PostLoadTask)
                         {
                             case "SelectGuildChannelTask":
                                 App.SelectGuildChannel(((App.GuildChannelSelectArgs)App.PostLoadTaskArgs).GuildId, ((App.GuildChannelSelectArgs)App.PostLoadTaskArgs).ChannelId);
                                 break;
                         }
                     }
                     //Check version number, and if it's different from before, open the what's new page
                     Package package = Package.Current;
                     PackageId packageId = package.Id;
                     string version = packageId.Version.Build.ToString()+packageId.Version.Major.ToString()+packageId.Version.Minor.ToString();
                     if (Storage.Settings.lastVerison == "0")
                     {
                         Storage.Settings.lastVerison = version;
                         Storage.SaveAppSettings();
                         App.NavigateToAbout(true);
                     }
                     else if (Storage.Settings.lastVerison != version)
                     {
                         Storage.Settings.lastVerison = version;
                         Storage.SaveAppSettings();
                         App.NavigateToAbout(true);
                     }
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

        private async void App_UserStatusChangedHandler(object sender, App.UserStatusChangedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if(e.Settings.GuildOrder != null)
                    {
                        int position = 1;
                        foreach (var guild in e.Settings.GuildOrder)
                        {
                            var item = ServerList.Items.FirstOrDefault(x => (x as GuildManager.SimpleGuild).Id == guild);
                            if (item == null) return;
                            if (ServerList.Items.IndexOf(item) != position)
                            {
                                ServerList.Items.Remove(item);
                                ServerList.Items.Insert(position, item);
                            }
                            position++;
                        }
                        position = 0;
                    }

                        if (e.Settings.Status != null)
                        {
                            if (e.Settings.Status != "invisible")
                            {
                                UserStatusIndicator.Fill = (SolidColorBrush)App.Current.Resources[e.Settings.Status];
                            } else
                            {
                                UserStatusIndicator.Fill = (SolidColorBrush)App.Current.Resources["offline"];
                            }
                            switch (e.Settings.Status)
                            {
                                case "online":
                                    UserStatusOnline.IsChecked = true;
                                    break;
                                case "idle":
                                    UserStatusIdle.IsChecked = true;
                                    break;
                                case "dnd":
                                    UserStatusDND.IsChecked = true;
                                    break;
                                case "invisible":
                                    UserStatusInvisible.IsChecked = true;
                                    break;
                            }
                        }
                });
        }

        #region Messages
        private async void App_MessageCreatedHandler(object sender, App.MessageCreatedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     var lastMsg = MessageList.Items.LastOrDefault() as MessageManager.MessageContainer;
                     if (e.Message.User.Id == LocalState.CurrentUser.Id)
                     {
                         if (lastMsg.Pending)
                         {
                             lastMsg.Message = lastMsg.Message.Value.MergePending(e.Message);
                             if (lastMsg.Message.Value.User.Id == null)
                             {
                                 lastMsg.Message.Value.SetUser(LocalModels.LocalState.CurrentUser);
                             }
                             lastMsg.Pending = false;
                         }
                     } else
                     {
                         MessageList.Items.Add(MessageManager.MakeMessage(e.Message));
                     }
                     App.MarkMessageAsRead(e.Message.Id, App.CurrentChannelId);
                     if (Storage.Settings.Vibrate && e.Message.User.Id!=LocalState.CurrentUser.Id)
                     {
                         var vibrationDuration = TimeSpan.FromMilliseconds(200);
                         if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification"))
                         {
                             var phonevibrate = Windows.Phone.Devices.Notification.VibrationDevice.GetDefault();
                             phonevibrate.Vibrate(vibrationDuration);
                         }
                         //This will be for another time, it clearly isn't working right now
                        /* var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault();
                         if(gamepad!=null)
                         {
                             GamepadVibration vibration = new GamepadVibration();
                             await Task.Run(async () =>
                             {
                                 vibration.RightMotor = 0.5;
                                 gamepad.Vibration = vibration;
                                 await Task.Delay(vibrationDuration);
                                 vibration.RightMotor = 0;
                             });
                         }*/
                     }
                     if (e.Message.TTS)
                     {
                         MediaElement mediaplayer = new MediaElement();
                         using (var speech = new SpeechSynthesizer())
                         {
                             speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Male);
                             string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" + e.Message.User.Username + "said" + e.Message.Content + "</speak>";
                             SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                             mediaplayer.SetSource(stream, stream.ContentType);
                             mediaplayer.Play();
                         }
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

        #region Members
        private async void App_MembersUpdatedHandler(object sender, EventArgs e)
        {
            int totalrolecounter = 0;
            int everyonecounter = LocalState.Guilds[App.CurrentGuildId].members.Count() - totalrolecounter;

            if (LocalState.Guilds[App.CurrentGuildId].Raw.Roles != null)
            {
                foreach (Role role in LocalState.Guilds[App.CurrentGuildId].Raw.Roles)
                {
                    Role roleAlt = role;
                    if (role.Hoist)
                    {
                        int rolecounter = 0;
                        foreach (GuildMember m in LocalState.Guilds[App.CurrentGuildId].members.Values)
                            if (m.Roles.FirstOrDefault() == role.Id) rolecounter++;
                        totalrolecounter += rolecounter;
                        roleAlt.MemberCount = rolecounter;
                    }
                    if (LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(role.Id))
                    {
                        LocalState.Guilds[App.CurrentGuildId].roles[role.Id] = roleAlt;
                    }
                    else
                    {
                        LocalState.Guilds[App.CurrentGuildId].roles.Add(role.Id, roleAlt);
                    }
                }
            }

            foreach (var member in LocalState.Guilds[App.CurrentGuildId].members.Values)
            {
                var m = new Member(member);
                if (m.Raw.Roles.FirstOrDefault() != null &&
                    LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(m.Raw.Roles.FirstOrDefault()) &&
                    LocalState.Guilds[App.CurrentGuildId].roles[m.Raw.Roles.FirstOrDefault()].Hoist)
                {
                    m.MemberDisplayedRole = MemberManager.GetRole(m.Raw.Roles.FirstOrDefault(), App.CurrentGuildId, everyonecounter);
                }
                else
                {
                    m.MemberDisplayedRole = MemberManager.GetRole(null, App.CurrentGuildId, everyonecounter);
                }
                if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                {
                    m.status = LocalState.PresenceDict[m.Raw.User.Id];
                }
                else
                {
                    m.status = new Presence() { Status = "offline", Game = null };
                }
                if (memberscvs.ContainsKey(m.Raw.User.Id))
                {
                    memberscvs.Remove(m.Raw.User.Id);
                }
                memberscvs.Add(m.Raw.User.Id, m);
            }
            try
            {
                var sortedMembers =
                    memberscvs.GroupBy(m => m.Value.MemberDisplayedRole).OrderByDescending(x => x.Key.Position);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                                // MembersCVS = new CollectionViewSource();
                                MembersCvs.Source = sortedMembers;
                    });
            }
            catch
            {

            }
        }
        #endregion

        #endregion

        #endregion

        #region UIEvents
        private void ToggleSplitView(object sender, RoutedEventArgs e)
        {
            sideDrawer.ToggleLeft();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int extrawidth = 0;
            if (UISize.CurrentState == Large | UISize.CurrentState == ExtraLarge)
                extrawidth = 240;
            ChannelHeader.MaxWidth = e.NewSize.Width - (72*3)+1+extrawidth;
        }

        private void UserStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (UserStatusOnline.IsChecked == true)
            {
                App.UpdatePresence("online");
            } else if (UserStatusIdle.IsChecked == true)
            {
                App.UpdatePresence("idle");
            } else if (UserStatusDND.IsChecked == true)
            {
                App.UpdatePresence("dnd");
            } else
            {
                App.UpdatePresence("invisible");
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            userFlyout.Hide();
            App.NavigateToSettings();
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServerList.SelectedItem != null)
            {
                var guildid = (ServerList.SelectedItem as GuildManager.SimpleGuild).Id;
                App.NavigateToGuild(guildid);
               
                sideDrawer.OpenLeft();
                Task.Run(() =>
                {
                    UserActivityManager.SwitchSession(guildid);
                }); 
            }
        }

        bool IgnoreChange = false;
        bool lastChangeProgrammatic = false;
        bool SelectionWasClicked = true;
        object previousSelection;
        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Verify if the last selection was navigated to (with a keyboard/controller) or actually clicked on

            //When selecting a category, we want to simulate ListView's Mode = Click, 
            //so we use IgnoreChange to immediately re-select the unselected item 
            //after having clicked on a category (without reloading anything)

            if (!lastChangeProgrammatic)
            {
                if (!IgnoreChange) //True if the last selection was a category, Voice channel
                {
                    if (SelectionWasClicked)
                    {
                        SelectionWasClicked = false; //clearly it was, but the next one will not necessarily be clicked. So set to false.

                        if (ChannelList.SelectedItem != null) //Called on clear
                        {
                            var channel = ChannelList.SelectedItem as ChannelManager.SimpleChannel;
                            if (channel.Type == 4) //CATEGORY
                            {
                                foreach (ChannelManager.SimpleChannel item in ChannelList.Items.Where(x => (x as ChannelManager.SimpleChannel).ParentId == channel.Id))
                                {
                                    if (item.Hidden)
                                        item.Hidden = false;
                                    else
                                        item.Hidden = true;
                                }
                                channel.Hidden = !channel.Hidden;
                                if (previousSelection == null)
                                    ChannelList.SelectedIndex = -1;
                                else
                                    ChannelList.SelectedItem = previousSelection;
                            }
                            else if (channel.Type == 2) //VOICE
                            {
                                IgnoreChange = true;
                                if (previousSelection == null)
                                    ChannelList.SelectedIndex = -1;
                                else
                                    ChannelList.SelectedItem = previousSelection;
                            }
                            else
                            {
                                sideDrawer.CloseLeft();
                                previousSelection = ChannelList.SelectedItem;
                                if (App.CurrentGuildIsDM)
                                {
                                    var cid = (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Id;
                                    App.NavigateToDMChannel(cid);
                                    Task.Run(() =>
                                    {
                                        UserActivityManager.SwitchSession(cid);
                                    });
                                }
                                else
                                {
                                    App.NavigateToGuildChannel(App.CurrentGuildId, (ChannelList.SelectedItem as ChannelManager.SimpleChannel).Id);
                                }
                            }
                        }
                    }

                }
                else
                {
                    IgnoreChange = false;
                }

            }
            else
            {
                lastChangeProgrammatic = false;
            }
        }
        private void ChannelList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectionWasClicked = true;
            if (e.ClickedItem == ChannelList.SelectedItem)
                //This is for xbox one, because when "clicking" on a channel, it is already selected
                ChannelList_SelectionChanged(null, null);
        }

        private void AddChannelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.NavigateToCreateChannel();
        }

        private void AddServer(object sender, RoutedEventArgs e)
        {
            App.NavigateToAddServer();
        }

        private void ServerNameButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuildEdit(App.CurrentGuildId);
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            App.CreateMessage(App.CurrentChannelId, MessageBox1.Text);
            
            MessageBox1.Text = "";
            MessageBox1.FocusTextBox();

            //Add a user activity for this channel:
            
            var guild = ServerList.SelectedItem as GuildManager.SimpleGuild;
            var channel = ChannelList.SelectedItem as ChannelManager.SimpleChannel;      
            Task.Run(async ()=>{
                if (App.CurrentGuildIsDM)
                {
                    await UserActivityManager.GenerateActivityAsync("DMs", channel.Name, channel.ImageURL, channel.Id,"");
                }
                else
                {
                    await UserActivityManager.GenerateActivityAsync(guild.Id, guild.Name, guild.ImageURL, channel.Id, "#"+channel.Name);
                }
            });
            
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
            sideDrawer.ToggleRight();
        }

        private void NavToAbout(object sender, RoutedEventArgs e)
        {
            App.NavigateToAbout();
        }

        private void NavToIAPs(object sender, RoutedEventArgs e)
        {
            App.NavigateToIAP();
        }

        private void ChannelHeader_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!App.CurrentGuildIsDM)
            {
                App.NavigateToChannelTopic(LocalState.Guilds[App.CurrentGuildId].channels[App.CurrentChannelId].raw);
            }
        }

        private void ScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }
        #endregion

        public Dictionary<string, Member> memberscvs = new Dictionary<string, Member>();

        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            messageStacker = sender as ItemsStackPanel;
        }

        private void WhatsNewClick(object sender, RoutedEventArgs e)
        {
            App.NavigateToAbout(true);
        }

        private void OpenFriendPanel(object sender, TappedRoutedEventArgs e)
        {
            ClearMessageArea();
            FriendsItem.IsSelected = true;
            ChannelList.SelectedIndex = -1;
            friendPanel.Visibility = Visibility.Visible;
            sideDrawer.CloseLeft();
        }

        private void HideBadge_Completed(object sender, object e)
        {

        }

        private void cmdBar_Opening(object sender, object e)
        {
            ChannelTopic.LineHeight = 12;
        }

        private void cmdBar_Closing(object sender, object e)
        {
            ChannelTopic.LineHeight = 24;
        }
    }
}
