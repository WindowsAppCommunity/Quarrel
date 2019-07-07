using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Connectivity;
using Windows.Phone.Devices.Notification;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using DiscordAPI.API.Channel.Models;
using Gma.DataStructures.StringSearch;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Debug = System.Diagnostics.Debug;
using EditChannel = Quarrel.SubPages.EditChannel;
using Guild = DiscordAPI.SharedModels.Guild;
using GuildChannel = Quarrel.LocalModels.GuildChannel;
using GuildSetting = DiscordAPI.SharedModels.GuildSetting;
using User = DiscordAPI.SharedModels.User;
using UserProfile = Quarrel.SubPages.UserProfile;
using System.Collections.ObjectModel;
using DiscordAPI.API.Game;
using DiscordAPI.API.Guild.Models;
using DiscordAPI.API.User.Models;
using Quarrel.Classes;
using Quarrel.Controls;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Gateway.DownstreamEvents;
using Quarrel.LocalModels;
using Quarrel.Managers;
using Quarrel.MarkdownTextBlock;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;
using Quarrel.SubPages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quarrel
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _autoselectchannel = "";
        private string _autoselectchannelcontent;
        private bool _autoselectchannelcontentsend;
        private Tuple<string, string> _currentPage = new Tuple<string, string>(null, null);


        private readonly DispatcherTimer _networkCheckTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};

        private bool _prevshowabove;
        private bool _prevshowbelow;
        private string _setupArgs = "";
        private bool AtBottom = false;
        private bool AtTop = false;
        private Rectangle cmdBarShadow;
        private readonly LoadingStack loadingStack = new LoadingStack();

        public GroupedObservableCollection<HoistRole, Member> memberscvs;
        public ObservableCollection<SimpleChannel> channelCollection = new ObservableCollection<SimpleChannel>();

        private readonly Stack<Tuple<string, string>> navigationHistory = new Stack<Tuple<string, string>>();

        private ExtendedExecutionSession session;

        public MainPage()
        {
            InitializeComponent();
            if (!App.IsDesktop) TitleBarHolder.Visibility = Visibility.Collapsed;
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            App.WentOffline += App_WentOffline;
            if (GatewayManager.Gateway != null)
            {
                GatewayManager.Gateway.GatewayClosed += Gateway_GatewayClosed;
                GatewayManager.Gateway.Resumed += Gateway_Resumed;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //sideDrawer.SetupInteraction(cmdBar);
            _setupArgs = e.Parameter as string;
            App.SetupMainPage += Setup;
            //PCAd.Width = MobileAd.Width = Ad.Width = 300;
            //PCAd.Height = MobileAd.Height = Ad.Height = 50;
            PCAd.ApplicationId = MobileAd.ApplicationId = Ad.ApplicationId = "9nbrwj777c8r";
            PCAd.AdUnitId = MobileAd.AdUnitId = Ad.AdUnitId = "1100023969";
            base.OnNavigatedTo(e);
            sideDrawer.SetupInteraction();
            loadingStack.FinishedLoading += LoadingStack_FinishedLoading;
            loadingStack.LoaderChanged += LoadingStack_LoaderChanged;

#if DEBUG
            AppBarButton debugPageButton = new AppBarButton();
            debugPageButton.Click += (sender, arg) =>
              {
                  SubFrameNavigator(typeof(SubPages.Debug));
              };
            debugPageButton.Label = "Debug stuff";
            cmdBar.SecondaryCommands.Add(debugPageButton);
#endif
        }

        private void LoadingStack_LoaderChanged(object sender, LoadingStack.Loader e)
        {
            Loading.Status = e.Status;
        }

        private void LoadingStack_FinishedLoading(object sender, EventArgs e)
        {
            Loading.Hide(true);
        }


        private void App_ConnectedToAppService(object sender, EventArgs e)
        {
            App._appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
            App._appServiceConnection.ServiceClosed += OnAppServicesClosed;
        }

        private async void OnAppServiceRequestReceived(AppServiceConnection sender,
            AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral deferral = args.GetDeferral();
            string content = "";
            if (args.Request.Message.ContainsKey("ConnectionUpdate"))
                content = args.Request.Message["ConnectionUpdate"].ToString();
            else if (args.Request.Message.ContainsKey("SET_ACTIVITY")) content = args.Request.Message["SET_ACTIVITY"].ToString();
            
            //MessageDialog md = new MessageDialog(content);
            //await md.ShowAsync();

            GatewayManager.Gateway.UpdateStatus(LocalState.CurrentUserPresence.Status, null, JsonConvert.DeserializeObject<Game>(content));

            ValueSet valueSet = new ValueSet();
            valueSet.Add("response", "success");
            await args.Request.SendResponseAsync(valueSet);
            deferral.Complete();
        }

        private void OnAppServicesClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            App._appServiceDeferral.Complete();
        }

        public async void Setup(object o, EventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //Reset everything, for when accounts are being switched
                ServerList.Items.Clear();
                //Setup UI
                MediumTrigger.MinWindowWidth = Storage.Settings.RespUiM;
                LargeTrigger.MinWindowWidth = Storage.Settings.RespUiL;
                ExtraLargeTrigger.MinWindowWidth = Storage.Settings.RespUiXl;
                TransitionCollection collection = new TransitionCollection();
                NavigationThemeTransition theme = new NavigationThemeTransition();
                DrillInNavigationTransitionInfo info = new DrillInNavigationTransitionInfo();
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
                    //ServerList.Padding = new Thickness(0, 84, 0, 48);
                    //ChannelList.Padding = new Thickness(0, 84, 0, 48);
                    ServerScrollviewer.Margin = new Thickness(0, 42, 0, 48);
                    ChannelScrollviewer.Margin = new Thickness(0, 42, 0, 0);
                    MembersListView.Margin = new Thickness(0, 48, 0, 48);

                    CinematicChannelName.Visibility = Visibility.Visible;
                    //CineGuildNameBTN.Visibility = Visibility.Visible;
                    //ServerNameButton.Visibility = Visibility.Collapsed;
                    friendPanel.Margin = new Thickness(0, 42, 0, 0);
                    MessageArea.Margin = new Thickness(0);
                    CinematicMask1.Visibility = Visibility.Visible;
                    ControllerHints.Visibility = Visibility.Visible;

                    //Set Controller Hint virtualkey
                    LBumperHint.Key = VirtualKey.GamepadLeftShoulder;
                    RBumperHint.Key = VirtualKey.GamepadRightShoulder;
                    //SelectHint.Key = VirtualKey.GamePad;
                    MenuHint.Key = VirtualKey.GamepadMenu;

                    if (App.ShowAds) XBOXAd.Visibility = Visibility.Visible;
                    PCAd.Visibility = Visibility.Collapsed;
                    Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
                    sideDrawer.DrawOpenedLeft += SideDrawer_DrawOpenedLeft;
                    sideDrawer.DrawOpenedRight += SideDrawer_DrawOpenedRight;
                    sideDrawer.DrawsClosed += SideDrawer_DrawsClosed;
                    SubFrame.FocusDisengaged += SubFrame_FocusDisengaged;
                    userButton.IsTabStop = false;
                }
                else
                {
                    ServerScrollviewer.Margin = new Thickness(0, 0, 0, 0);
                }

                //Setup BackButton
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
                //Setup Controller input
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
                Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
                Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
                //Setup MessageList infinite scroll

                if (!Storage.Settings.CustomBG) BackgroundImage.Visibility = Visibility.Collapsed;

                if (App.DontLogin) return;

                //Hook up the login Event
                App.LoggingInHandler += App_LoggingInHandlerAsync;

                UISize.CurrentStateChanged += UISize_CurrentStateChanged;
                //Verify if a token exists, if not navigate to login page
                if (App.LoggedIn() == false)
                    SubFrameNavigator(typeof(LogScreen));
                else
                    App.LogIn();
            });
        }

        private void SubFrame_FocusDisengaged(Control sender, FocusDisengagedEventArgs args)
        {
            App.SubpageClose();
        }

        private void UISize_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == Large || e.NewState == ExtraLarge)
            {
                if (App.ShowAds && !App.CinematicMode)
                {
                    PCAd.Visibility = Visibility.Visible;
                    MobileAd.Visibility = Visibility.Collapsed;
                }

                if (content.Children.Contains(cmdBar))
                {
                    content.Children.Remove(cmdBar);
                    MessageAreaCMD.Children.Add(cmdBar);
                }

                if (e.NewState == Large)
                {
                    MemberToggle.Visibility = Visibility.Visible;
                    //ShowMemberToggle.Begin();
                    burgerButton.Visibility = Visibility.Collapsed;
                }
                else if (e.NewState == ExtraLarge)
                {
                    MemberToggle.Visibility = Visibility.Collapsed;
                    //HideMemberToggle.Begin();
                    burgerButton.Visibility = Visibility.Collapsed;
                }

                cmdBar.Background = (Brush) Application.Current.Resources["AcrylicMessageBackground"];
                cmdBarShadow.Visibility = Visibility.Visible;
            }
            else
            {
                if (MessageAreaCMD.Children.Contains(cmdBar))
                {
                    MessageAreaCMD.Children.Remove(cmdBar);
                    content.Children.Add(cmdBar);
                }

                if (App.ShowAds && !App.CinematicMode)
                {
                    PCAd.Visibility = Visibility.Collapsed;
                    MobileAd.Visibility = Visibility.Visible;
                }

                MemberToggle.Visibility = Visibility.Visible;
                //ShowMemberToggle.Begin();
                burgerButton.Visibility = Visibility.Visible;

                cmdBar.Background = (Brush) Application.Current.Resources["AcrylicCommandBarBackground"];
                //  cmdBarShadow.Visibility = Visibility.Collapsed;
            }

            if (!App.ShowAds || App.CinematicMode)
            {
                PCAd.Visibility = Visibility.Collapsed;
                MobileAd.Visibility = Visibility.Collapsed;
            }
        }

        private void ServerScrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            RefreshVisibilityIndicators();
        }

        private void RefreshVisibilityIndicators()
        {
            bool showabove = false;
            bool showbelow = false;

            foreach (SimpleGuild sg in ServerList.Items)
                if (sg.NotificationCount > 0)
                {
                    VisibilityPosition pos = GetVisibilityPosition((ListViewItem) ServerList.ContainerFromItem(sg),
                        ServerScrollviewer);
                    if (pos == VisibilityPosition.Above)
                        showabove = true;
                    else if (pos == VisibilityPosition.Below)
                        showbelow = true;
                }

            if (showabove && !_prevshowabove)
                NewAboveIndicator.Fade(0.8f, 200).Start();
            else if (_prevshowabove != showabove)
                NewAboveIndicator.Fade(0, 200).Start();
            if (showbelow && !_prevshowbelow)
                NewBelowIndicator.Fade(0.8f, 200).Start();
            else if (_prevshowbelow != showbelow)
                NewBelowIndicator.Fade(0, 200).Start();

            _prevshowbelow = showbelow;
            _prevshowabove = showabove;
        }

        private VisibilityPosition GetVisibilityPosition(FrameworkElement element, FrameworkElement container)
        {
            if (element == null || container == null)
                return VisibilityPosition.Hidden;

            if (element.Visibility != Visibility.Visible)
                return VisibilityPosition.Hidden;

            Rect elementBounds = element.TransformToVisual(container)
                .TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            Rect containerBounds = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            if (elementBounds.Bottom < 4)
                return VisibilityPosition.Above;
            if (elementBounds.Top > containerBounds.Bottom - 32)
                return VisibilityPosition.Below;
            return VisibilityPosition.Visible;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if (SubFrame.Visibility == Visibility.Visible && App.shareop == null
            ) //if the app was opened as a share target, disable back navigation
            {
                App.SubpageClose();
            }
            else
            {
                if (navigationHistory.Count > 0)
                {
                    Tuple<string, string> page = navigationHistory.Pop();
                    if (page.Item1 != null)
                    {
                        App.SelectGuildChannel(page.Item1, page.Item2, null, false, true);
                    }
                    else
                    {
                        if (page.Item2 != null)
                            App.NavigateToDMChannel(page.Item2, null, false, true);
                        else
                            App.NavigateToDMChannel(null, null, false, true);
                    }
                }
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
            App.NavigateToRemoveGroupUserHandler += App_NavigateToRemoveGroupUserHandler;
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
            App.NavigateToAccountSettingsHandler += App_NavigateToAccountSettingsHandler;
            App.NavigateToAboutHandler += App_NavigateToAboutHandler;
            App.NavigateToAddServerHandler += App_NavigateToAddServerHandler;
            App.NavigateToMessageEditorHandler += App_NavigateToMessageEditorHandler;
            App.NavigateToIAPSHandler += App_NavigateToIAPSHandler;
            App.ShowSubFrameHandler += App_ShowSubFrameHandler;
            //Flyouts
            App.MenuHandler += App_MenuHandler;
            App.ShowMemberFlyoutHandler += App_ShowMemberFlyoutHandler;
            App.ShowGameFlyoutHandler += App_ShowGameFlyoutHandler;
            //Link
            App.LinkClicked += App_LinkClicked;
            //API
            App.FlashMentionHandler += App_FlashMentionHandler;
            typingCooldown.Tick += TypingCooldown_Tick;
            App.StartTypingHandler += App_StartTypingHandler;
            App.AddFriendHandler += App_AddFriendHandler;
            App.BlockUserHandler += App_BlockUserHandler;
            App.MarkMessageAsReadHandler += App_MarkMessageAsReadHandler;
            App.MarkCategoryAsReadHandler += App_MarkCategoryAsReadHandler;
            App.MarkChannelAsReadHandler += App_MarkChannelAsReadHandler;
            App.MarkGuildAsReadHandler += App_MarkGuildAsReadHandler;
            App.MuteChannelHandler += App_MuteChannelHandler;
            App.MuteGuildHandler += App_MuteGuildHandler;
            App.RemoveFriendHandler += App_RemoveFriendHandler;
            App.UpdatePresenceHandler += App_UpdatePresenceHandler;
            App.VoiceConnectHandler += App_VoiceConnectHandler;
            App.GuildSyncedHandler += App_GuildSyncedHandler;
            App.PresenceUpdatedHandler += App_PresenceUpdatedHandler;
            //DM
            App.DMCreatedHandler += App_DMCreatedHandler;
            App.DMDeletedHandler += App_DMDeletedHandler;
            App.DMUpdatePosHandler += App_DMUpdatePosHandler;
            //UpdateUI
            App.ReadyRecievedHandler += App_ReadyRecievedHandler;
            App.TypingHandler += App_TypingHandler;
            App.UpdateUnreadIndicatorsHandler += App_UpdateUnreadIndicatorsHandler;
            App.UserStatusChangedHandler += App_UserStatusChangedHandler;
            //UpdateUI-Channels
            App.GuildChannelCreatedHandler += App_GuildChannelCreatedHandler;
            App.GuildChannelDeletedHandler += App_GuildChannelDeletedHandler;
            App.GuildChannelUpdatedHandler += App_GuildChannelUpdatedHandler;
            //UpdateUI-Guilds
            App.GuildCreatedHandler += App_GuildCreatedHandler;
            App.GuildDeletedHandler += App_GuildDeletedHandler;
            App.GuildUpdatedHandler += App_GuildUpdatedHandler;
            //UpdateUI-Members
            App.MembersUpdatedHandler += App_MembersUpdatedHandler;

            //Auto selects
            App.SelectGuildChannelHandler += App_SelectGuildChannelHandler;
            App.SelectDMChannelHandler += App_SelectDMChannelHandler;

            App.ToggleCOModeHandler += App_ToggleCOModeHandler;

            App.ConnectedToAppService += App_ConnectedToAppService;

            App.VirtualKeyHitHandler += App_VirtualKeyHitHandler;

            _networkCheckTimer.Tick += _networkCheckTimer_Tick;
        }

        private void App_VirtualKeyHitHandler(object sender, App.KeyHitArgs e)
        {

            if (e.Key == VirtualKey.Shift)
            {
                //     MessageBox1.ShiftDown();
            }
            /*
            else if (args.VirtualKey == VirtualKey.S)
            {
                if (args.KeyStatus.IsKeyReleased && CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Control) != CoreVirtualKeyStates.None)
                {
                    SubFrameNavigator(typeof(SubPages.DiscordStatus));
                }
                else
                {
                    args.Handled = true;
                }
            }*/
            else if (e.Key == VirtualKey.GamepadLeftThumbstickLeft)
            {
                // args.Handled = true;
            }
            else if (e.Key == VirtualKey.GamepadLeftThumbstickRight)
            {
                //  args.Handled = true;
            }
            else if (e.Key == VirtualKey.GamepadLeftThumbstickUp)
            {
                // args.Handled = true;
            }
            else if (e.Key == VirtualKey.GamepadLeftThumbstickDown)
            {
                //  args.Handled = true;
                //  ScrollviewerFromGamepad();
            }
            else if (e.Key == VirtualKey.GamepadLeftShoulder)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                if (!e.Released)
                    sideDrawer.ToggleLeft();

                if (e.Released)
                {
                    LBumperHint.Release();
                }
                else
                {
                    LBumperHint.Press();
                }
            }
            else if (e.Key == VirtualKey.GamepadRightShoulder)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                if (!e.Released)
                    sideDrawer.ToggleRight();

                if (e.Released)
                {
                    RBumperHint.Release();
                }
                else
                {
                    RBumperHint.Press();
                }
            }
            else if (e.Key == VirtualKey.GamepadView)
            {
                if (SubFrame.Visibility == Visibility.Visible) return;
                if (e.Released)
                {
                    MenuHint.Release();
                    MenuHint.ContextFlyout.ShowAt(MenuHint);
                }
                else
                {
                    MenuHint.Press();
                }
            }
        }

        private async void _networkCheckTimer_Tick(object sender, object e)
        {
            if (GatewayManager.Gateway.ConnectedSocket == false)
                if (NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() ==
                    NetworkConnectivityLevel.InternetAccess)
                {
                    try
                    {
                        await GatewayManager.Gateway.ResumeAsync();
                    }
                    catch
                    {
                        App.CheckOnline();
                    }

                    if (GatewayManager.Gateway.ConnectedSocket)
                        _networkCheckTimer.Stop();
                    else
                        App.CheckOnline();
                }
        }

        private async void Gateway_Resumed(object sender, GatewayEventArgs<Resumed> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    if (_networkCheckTimer.IsEnabled) _networkCheckTimer.Stop();
                    await DisconnectedMask.Fade(0, 300).StartAsync();
                    DisconnectedMask.Visibility = Visibility.Collapsed;
                });
        }

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            if (NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() ==
                NetworkConnectivityLevel.InternetAccess)
                if (GatewayManager.Gateway != null && GatewayManager.Gateway.ConnectedSocket == false)
                    await GatewayManager.Gateway.ResumeAsync();
        }

        private async void Gateway_GatewayClosed(object sender, Windows.Networking.Sockets.WebSocketClosedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (e != null && e.Code == 4004)
                    {
                        //Authentication failed
                        loadingStack.Clear();
                        SubFrameNavigator(typeof(LogScreen));
                        Loading.Hide(true);
                        MessageDialog md = new MessageDialog("Sorry, but we couldn't connect to Discord's servers with your login information!", "Authentication failed");

                        
                    }
                    else if (DisconnectedMask.Opacity != 1)
                    {
                        DisconnectedMask.Opacity = 0;
                        DisconnectedMask.Visibility = Visibility.Visible;
                        DisconnectedMask.Fade(1, 300).Start();
                        _networkCheckTimer.Start();
                        loadingStack.Loaded("GatewayConnecting");
                    }
                });
        }

        private async void Gateway_GuildMemberRemoved(object sender, GatewayEventArgs<GuildMemberRemove> e)
        {
            if (App.CurrentGuildId != e.EventData.guildId) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { memberscvs.Remove(FindMember(e.EventData.User.Id)); });
        }

        private void Gateway_GuildMemberAdded(object sender, GatewayEventArgs<GuildMemberAdd> e)
        {
            if (App.CurrentGuildId != e.EventData.guildId) return;
            GuildMemberAdd member = e.EventData;
            Member m = new Member(new GuildMember
            {
                Deaf = e.EventData.Deaf,
                JoinedAt = e.EventData.JoinedAt,
                Mute = e.EventData.Mute,
                Nick = e.EventData.Nick,
                Roles = e.EventData.Roles,
                User = e.EventData.User
            });

            AddToMembersCvs(m);
        }

        private async void AddToMembersCvs(Member m, bool dm = false)
        {
            if (m.Raw.Roles != null)
                m.Raw.Roles = m.Raw.Roles.TakeWhile(x => LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(x))
                    .OrderByDescending(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Position);

            //Set it to first Hoist Role or everyone if null
            if (dm)
                m.MemberHoistRole = new HoistRole("MEMBERS", 0, "MEMBERS", 0, -1);
            else
                m.MemberHoistRole =
                    MemberManager.GetRole(
                        m.Raw.Roles.FirstOrDefault(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Hoist),
                        App.CurrentGuildId);

            if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                m.status = LocalState.PresenceDict[m.Raw.User.Id];
            else
                m.status = new Presence {Status = "offline", Game = null};
            if (m.Raw.Nick != null)
                m.DisplayName = m.Raw.Nick;
            else
                m.DisplayName = m.Raw.User.Username;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (memberscvs == null) return;
                    memberscvs.Add(m);
                });
        }

        private Member FindMember(string id)
        {
            if (memberscvs != null && memberscvs.RoleIndexer.ContainsKey(id))
            {
                HoistRole key = memberscvs.RoleIndexer[id];
                Grouping<HoistRole, Member> group = null;
                foreach (Grouping<HoistRole, Member> g in memberscvs)
                    if (key.Id == g.Group.Id)
                        group = g;
                if (group != null)
                    return group.FirstOrDefault(x => x.Raw.User.Id == id);
                return null;
            }

            return null;
        }

        private async void Gateway_GuildMemberUpdated(object sender, GatewayEventArgs<GuildMemberUpdate> e)
        {
            if (App.CurrentGuildId != e.EventData.guildId) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Member member = FindMember(e.EventData.User.Id);
                if (member == null) return;
                member.Raw.Nick = e.EventData.Nick;
                if (e.EventData.Nick != null)
                    member.DisplayName = e.EventData.Nick;
                else
                    member.DisplayName = member.Raw.User.Username;

                member.Raw.User = e.EventData.User;
                member.Raw.Roles = e.EventData.Roles;
                // member.Raw.Nick = e.EventData.Nick;
                HoistRole previoushoistrole = new HoistRole(member.MemberHoistRole.Id, member.MemberHoistRole.Position,
                    member.MemberHoistRole.Name, member.MemberHoistRole.Membercount, member.MemberHoistRole.Brush);
                member.MemberHoistRole =
                    MemberManager.GetRole(
                        LocalState.Guilds[App.CurrentGuildId].GetHighestHoistRoleId(e.EventData.Roles),
                        App.CurrentGuildId);
                if (!member.MemberHoistRole.Equals(previoushoistrole))
                    memberscvs.ChangeKey(member, previoushoistrole, member.MemberHoistRole);
            });
        }

        private async void App_PresenceUpdatedHandler(object sender, App.PresenceUpdatedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (App.CurrentGuildIsDM)
                    foreach (SimpleChannel channel in channelCollection)
                        if (channel.UserId != null && channel.UserId == e.UserId)
                            channel.UserStatus = e.Presence;
                //if the memberscvs isn't null, and either the current guild is DMs or the currentguild isn't null and contains the member
                if (memberscvs != null && (App.CurrentGuildIsDM || App.CurrentGuildId != null &&
                                           LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(e.UserId)))
                {
                    Member member = FindMember(e.UserId);
                    if (member == null)
                    {
                        if (App.CurrentGuildId == null) return;
                        if (e.Presence.Status == "offline" || e.Presence.Status == "invisible") return;
                        if (!LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(e.UserId)) return;

                        member = new Member(LocalState.Guilds[App.CurrentGuildId].members[e.UserId]);
                        member.MemberHoistRole = MemberManager.GetRole(
                            member.Raw.Roles.FirstOrDefault(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Hoist),
                            App.CurrentGuildId);
                        if (!string.IsNullOrEmpty(member.Raw.Nick))
                            member.DisplayName = member.Raw.Nick;
                        else
                            member.DisplayName = member.Raw.User.Username;
                        member.status = e.Presence;
                        memberscvs.Add(member);
                    }
                    else if (e.Presence.Status != "offline" && e.Presence.Status != "invisible")
                    {
                        member.status = e.Presence;
                    }
                    else
                    {
                        member.MemberHoistRole.Membercount--;
                        memberscvs.Remove(member);
                    }
                }
            });
            //   var member = memberscvs.Items.FirstOrDefault();

            // if (memberscvs.Item)
            //   ((Member)memberscvs[e.UserId]).status = e.Presence;
            /*  await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                     () =>
                     {
                         MembersCvs.Source = memberscvs;
                     });*/
        }

        private async void App_GuildUpdatedHandler(object sender, Guild e)
        {
            //update localstate guilds
            LocalState.Guilds[e.Id].Raw = e;
            //update icon
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    foreach (SimpleGuild guild in ServerList.Items)
                        if (guild.Id == e.Id)
                        {
                            if (!string.IsNullOrEmpty(e.Icon))
                                guild.ImageURL = "https://cdn.discordapp.com/icons/" + e.Id + "/" + e.Icon + ".png";

                            else
                                guild.ImageURL = "empty";
                        }
                });
        }

        private async void App_FlashMentionHandler(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (MentionFlasherStoryboard.GetCurrentState() != ClockState.Stopped)
                        MentionFlasherStoryboard.Stop();
                    MentionFlasherStoryboard.Begin();
                });
        }

        private async void App_GuildDeletedHandler(object sender, App.GuildDeletedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    foreach (SimpleGuild guild in ServerList.Items)
                        if (guild.Id == e.GuildId)
                        {
                            if (App.CurrentGuildId == e.GuildId)
                                ServerList.SelectedIndex = 0;

                            ServerList.Items.Remove(guild);
                            if (LocalState.Guilds.ContainsKey(e.GuildId))
                                LocalState.Guilds.Remove(e.GuildId);
                            break;
                        }
                });
        }
        

        private void App_SelectGuildChannelHandler(object sender, App.GuildChannelSelectArgs e)
        {
            Ad.Visibility = Visibility.Collapsed;
            string guildid = e.GuildId;
            string channelid = e.ChannelId;
            if (e.Navigate)
            {
                _autoselectchannelcontent = e.MessageContent;
                _autoselectchannelcontentsend = e.Send;

                if (guildid == "friendrequests")
                    friendPanel.NavigateToFriendRequests();
                else
                    foreach (SimpleGuild g in ServerList.Items)
                        if (g.Id == guildid)
                        {
                            _autoselectchannelcontent = e.MessageContent;
                            _autoselectchannel = channelid;
                            ServerSelectionWasClicked =
                                true; //It wasn't actually, hehehe. Let me teach you a lesson in trickery, this is going down in history...
                            ServerList.SelectedItem = g;
                            if (App.CurrentGuildId == guildid || (App.CurrentGuildIsDM && guildid == "@me"))
                            {
                                ServerList_SelectionChanged(null, null);
                            }
                        }
            }
            else if (e.Send)
            {
                App.CreateMessage(e.ChannelId, e.MessageContent);
            }
        }

        private async void App_SelectDMChannelHandler(object sender, App.DMChannelSelectArgs e)
        {
            if (e.UserId != null)
            {
                string channelid = null;
                foreach (KeyValuePair<string, DirectMessageChannel> dm in LocalState.DMs)
                    if (dm.Value.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == e.UserId)
                        channelid = dm.Value.Id;
                if (channelid == null)
                    channelid = (await RESTCalls.CreateDM(new CreateDM
                        {Recipients = new List<string> {e.UserId}.AsEnumerable()})).Id;

                App.SelectGuildChannel("@me", channelid, e.Message, e.Send, true);
            }
            else
            {
                App.SelectGuildChannel("@me", e.ChannelId, e.Message, e.Send, true);
            }
        }

        public void ClearData()
        {
            LocalState.CurrentUser = new User();
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
            App.SubpageClosedHandler -= App_SubpageClosedHandler;
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
            App.ShowSubFrameHandler -= App_ShowSubFrameHandler;
            //Flyouts
            App.MenuHandler -= App_MenuHandler;
            App.ShowMemberFlyoutHandler -= App_ShowMemberFlyoutHandler;
            App.ShowGameFlyoutHandler -= App_ShowGameFlyoutHandler;
            //Link
            App.LinkClicked -= App_LinkClicked;
            //API
            App.FlashMentionHandler -= App_FlashMentionHandler;
            typingCooldown.Tick -= TypingCooldown_Tick;
            App.StartTypingHandler -= App_StartTypingHandler;
            App.AddFriendHandler -= App_AddFriendHandler;
            App.BlockUserHandler -= App_BlockUserHandler;
            App.MarkMessageAsReadHandler -= App_MarkMessageAsReadHandler;
            App.MarkChannelAsReadHandler -= App_MarkChannelAsReadHandler;
            App.MarkGuildAsReadHandler -= App_MarkGuildAsReadHandler;
            App.MuteChannelHandler -= App_MuteChannelHandler;
            App.MuteGuildHandler -= App_MuteGuildHandler;
            App.RemoveFriendHandler -= App_RemoveFriendHandler;
            App.UpdatePresenceHandler -= App_UpdatePresenceHandler;
            App.VoiceConnectHandler -= App_VoiceConnectHandler;
            App.GuildSyncedHandler -= App_GuildSyncedHandler;
            App.PresenceUpdatedHandler -= App_PresenceUpdatedHandler;
            //DM
            App.DMCreatedHandler -= App_DMCreatedHandler;
            App.DMDeletedHandler -= App_DMDeletedHandler;
            App.DMUpdatePosHandler -= App_DMUpdatePosHandler;
            //UpdateUI
            App.ReadyRecievedHandler -= App_ReadyRecievedHandler;
            App.TypingHandler -= App_TypingHandler;
            App.UpdateUnreadIndicatorsHandler -= App_UpdateUnreadIndicatorsHandler;
            App.UserStatusChangedHandler -= App_UserStatusChangedHandler;
            //UpdateUI-Channels
            App.GuildChannelCreatedHandler -= App_GuildChannelCreatedHandler;
            App.GuildChannelDeletedHandler -= App_GuildChannelDeletedHandler;
            App.GuildChannelUpdatedHandler -= App_GuildChannelUpdatedHandler;
            //UpdateUI-Guilds
            App.GuildCreatedHandler -= App_GuildCreatedHandler;
            App.GuildDeletedHandler -= App_GuildDeletedHandler;
            App.GuildUpdatedHandler -= App_GuildUpdatedHandler;
            //UpdateUI-Members
            App.MembersUpdatedHandler -= App_MembersUpdatedHandler;

            //Auto selects
            App.SelectGuildChannelHandler -= App_SelectGuildChannelHandler;

            App.ToggleCOModeHandler -= App_ToggleCOModeHandler;


            App.WentOffline -= App_WentOffline;


            ServerList.Items.Clear();
            channelCollection.Clear();
            MembersListView.Items.Clear();
        }

        private void App_WentOffline(object sender, StatusPageClasses.Index e)
        {
            loadingStack.Clear();
            SubFrameNavigator(typeof(Offline), e);
        }

        private async void BeginExtendedExecution()
        {
            ClearExtendedExecution();

            ExtendedExecutionSession newSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.Unspecified,
                Description = "Periodic update of live tile"
            };
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    session = newSession;
                    Console.WriteLine("Extened execution");
                    //periodicTimer = new Timer();
                    break;

                default:
                case ExtendedExecutionResult.Denied:
                    newSession.Dispose();
                    break;
            }
        }

        private void ClearExtendedExecution()
        {
            if (session != null)
            {
                session.Revoked -= SessionRevoked;
                session.Dispose();
                session = null;
            }
        }

        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        break;
                }

                ClearExtendedExecution();
            });
        }

        private void WhatsNewClick(object sender, RoutedEventArgs e)
        {
            App.NavigateToAbout(true);
        }

        private void OpenFriendPanel(object sender, TappedRoutedEventArgs e)
        {
            App.CurrentChannelId = null;
            MessageBody.Visibility = Visibility.Collapsed;
            PinnedMessags.Visibility = Visibility.Collapsed;
            FriendsItem.IsSelected = true;
            if (ChannelList.SelectedItem != null && ChannelList.SelectedItem is SimpleChannel channel)
                channel.IsSelected = false;
            ChannelList.SelectedIndex = -1;
            friendPanel.Visibility = Visibility.Visible;
            CallUser.Visibility = Visibility.Collapsed;
            ChannelName.Text = ChannelTopic.Text = "";
            if (App.CinematicMode) CinematicChannelName.Visibility = Visibility.Collapsed;
          //  MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
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


        //private void TextBlock_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    GatewayManager.Gateway.UpdateStatus("online", null, new Game() { Name = PlayingBox.Text });
        //}

        private void ServerList_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectHint.Show();
        }

        private void ServerList_LostFocus(object sender, RoutedEventArgs e)
        {
            // ChannelList.SelectedItem = channelCollection.FirstOrDefault(x => ((SimpleChannel)x).Id == App.CurrentChannelId);
        }

        private void sideDrawer_SecondaryLeftFocused_1(object sender, EventArgs e)
        {
            if (App.CinematicMode)
            {
                if (ChannelList.SelectedItem != null)
                {
                    ListViewItem item = (ListViewItem) ChannelList.ContainerFromItem(ChannelList.SelectedItem);
                    item.Focus(FocusState.Keyboard);
                }
                else
                {
                    ChannelList.Focus(FocusState.Keyboard);
                }
            }
        }

        private void ChannelList_GotFocus(object sender, RoutedEventArgs e)
        {
            // YHint.Show();
        }

        private void ChannelList_LostFocus(object sender, RoutedEventArgs e)
        {
            //ServerList.SelectedItem = ServerList.Items.FirstOrDefault(x => ((SimpleGuild)x).Id == App.CurrentGuildId);    
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            //Delete account
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            //Add account
            SubFrameNavigator(typeof(LogScreen));
        }

        private void AccountView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Switch account
            userFlyout.Hide();
            Storage.Settings.DefaultAccount = ((PasswordCredential) e.ClickedItem).UserName;
            Storage.SaveAppSettings();
            Loading.Show(true);
            Setup(null, null);
        }

        private async void App_ToggleCOModeHandler(object sender, EventArgs e)
        {
            if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default)
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            else
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            COVoice.Visibility = COVoice.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        #region KeyboardAccelorators

        private void KeyboardOpenPin(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            PinnedMessags.Flyout.ShowAt(PinnedMessags);
        }

        #endregion

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.Search));
        }

        private void COVoice_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }



        private void content_DragOver(object sender, DragEventArgs e)
        {
            if (App.CurrentChannelId != null)
            {
                e.AcceptedOperation = DataPackageOperation.Copy;

                DroppingRectangle.Fade(1, 300).Start();
                float cX = Convert.ToSingle(DroppingRectangle.ActualWidth / 2f);
                float cY = Convert.ToSingle(DroppingRectangle.ActualHeight / 2f);
                DroppingRectangle.Scale(1.05f, 1.05f, cX, cY, 0, 0).Start();
                DroppingRectangle.Scale(1f, 1f, cX, cY, 300).Start();
            }
        }

        private void content_DragLeave(object sender, DragEventArgs e)
        {
            DroppingRectangle.Fade(0, 300).Start();
            float cX = Convert.ToSingle(DroppingRectangle.ActualWidth / 2f);
            float cY = Convert.ToSingle(DroppingRectangle.ActualHeight / 2f);
            DroppingRectangle.Scale(1.05f, 1.05f, cX, cY, 300).Start();
        }

        private void content_Drop(object sender, DragEventArgs e)
        {
            if (App.CurrentChannelId != null)
            {
                SubFrameNavigator(typeof(ExtendedMessageEditor), e.DataView);
                DroppingRectangle.Fade(0, 300).Start();
                float cX = Convert.ToSingle(DroppingRectangle.ActualWidth / 2f);
                float cY = Convert.ToSingle(DroppingRectangle.ActualHeight / 2f);
                DroppingRectangle.Scale(1.05f, 1.05f, cX, cY, 300).Start();
            }
        }

        private void MembersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListViewItem memberItem = (ListViewItem) MembersListView.ContainerFromItem(e.ClickedItem);
            App.ShowMemberFlyout(memberItem, (e.ClickedItem as Member).Raw.User, false);
        }


        private void SubFrame_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SubFrame.Opacity == 1)
            {
                //NOPE, FUCK OFF, YOU'RE NOT ALLOWED TO LOSE FOCUS YOU USELESS INBRED CUMSTAIN
                object el = FocusManager.GetFocusedElement();
                if (el != null) Debug.WriteLine("It lost focus. Shit.");
            }
        }

        private void SubFrame_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void SubFrame_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            if (SubFrame.Opacity == 1 && !IsParentFrame(args.NewFocusedElement))
                if (args.OldFocusedElement.GetType() == typeof(Control))
                    ((Control) args.OldFocusedElement).Focus(FocusState.Keyboard);
        }

        private bool IsParentFrame(DependencyObject child)
        {
            //recursion recursion recursion recursion recursion recursion to figure out if one of the DependencyObject's parents is the SubFrame
            if (child == null || child.GetType() != typeof(Control)) return true;

            Control childc = (Control) child;
            if (childc.BaseUri == null) return true;
            if (childc.BaseUri.ToString().EndsWith("MainPage.xaml"))
                return false;
            return true;
        }

        private void UISize_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
        }

        private void cmdBarShadow_Loaded(object sender, RoutedEventArgs e)
        {
            cmdBarShadow = (Rectangle) sender;
            
            UISize_CurrentStateChanged(null, new VisualStateChangedEventArgs {NewState = UISize.CurrentState});
        }

        private void AcceptCall(object sender, RoutedEventArgs e)
        {
            try
            {
                App.ConnectToVoice(AcceptCallUI.Tag.ToString(), null, "@User", "");
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private async void DeclineCall(object sender, RoutedEventArgs e)
        {
            try
            {
                await RESTCalls.DeclineCall(AcceptCallUI.Tag.ToString());
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private void NavToDiscordStatus(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(DiscordStatus));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ((HyperlinkButton) sender).ContextFlyout.ShowAt((HyperlinkButton) sender);
        }

        //private void Flyout_Closed(object sender, object e)
        //{
        //    if (string.IsNullOrWhiteSpace(PlayingBox.Text))
        //        GatewayManager.Gateway.UpdateStatus(null, 0, null);
        //    else
        //        GatewayManager.Gateway.UpdateStatus(null, 0, new GameBase {Type = 0, Name = PlayingBox.Text});
        //}

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.AuditLog), App.CurrentGuildId);
        }

        private async void CreateInvite(object sender, RoutedEventArgs e)
        {
            //TODO: Create new "create invite" system
            Invite invite = await RESTCalls.CreateInvite(
                !string.IsNullOrEmpty(App.CurrentChannelId)
                    ? App.CurrentChannelId
                    : LocalState.CurrentGuild.channels.First().Value.raw.Id,
                new CreateInvite() {MaxAge = 86400, MaxUses = 0, Temporary = false});
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = "Invite Link",
                ConfirmMessage = "", //App.GetString("/Dialogs/Clipboard"),
                SubMessage = "",
                StartText = invite.String,
                PlaceHolderText = null,
                ConfirmRed = false,
                ReadOnly = true,
                CanBeFancy = false
                //args = App.CurrentGuildId,
                //function = //TODO: Copy to Clipboard
            });
        }

        private void AddChannelFlyout(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(CreateChannel));
        }

        private void LeaveGuildFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (LocalState.CurrentGuild.Raw.OwnerId == LocalState.CurrentUser.Id)
            {
                App.NavigateToDeleteServer(LocalState.CurrentGuild.Raw.Id);
            }
            else
            {
                App.NavigateToLeaveServer(LocalState.CurrentGuild.Raw.Id);
            }
        }
        
        private async void CallUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.CurrentGuildIsDM)
                {
                    await RESTCalls.StartCall(App.CurrentChannelId);
                    App.ConnectToVoice(App.CurrentChannelId, null, "@User", "");
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err);
            }
        }

        private enum VisibilityPosition
        {
            Visible,
            Above,
            Below,
            Hidden
        }

        #region AppEvents

        #region LogIn

        private async void App_LoggingInHandlerAsync(object sender, EventArgs e)
        {
            Loading.Show(false);
            SubFrameMask.Opacity = 0;
            loadingStack.Loading("LoggingIn", "LOGGING IN");

            IReadOnlyList<PasswordCredential> credentials = Storage.PasswordVault.FindAllByResource("Token");
            AccountView.Items.Clear();
            foreach (PasswordCredential cred in credentials)
                if (cred.UserName != Storage.Settings.DefaultAccount)
                    AccountView.Items.Add(cred);
            if (App.IsMobile) TitleBarHolder.Visibility = Visibility.Collapsed;
            if (App.LoggedIn())
            {
                loadingStack.Loading("GatewayConnecting", "CONNECTING");
                loadingStack.Loaded("LoggingIn");
                SetupEvents();
                if (App.Ready)
                    App_ReadyRecievedHandler(null, null);
                if (GatewayManager.Gateway != null)
                {
                    GatewayManager.Gateway.GatewayClosed += Gateway_GatewayClosed;
                    GatewayManager.Gateway.Resumed += Gateway_Resumed;
                    //Debug.Write(Windows.UI.Notifications.BadgeUpdateManager.GetTemplateContent(Windows.UI.Notifications.BadgeTemplateType.BadgeNumber).GetXml());
                    BeginExtendedExecution();
                    BackgroundTaskManager.TryRegisterBackgroundTask();
                    SubFrame.Visibility = Visibility.Collapsed;
                    if (LocalState.SupportedGames == null || LocalState.SupportedGames.Count == 0)
                    {
                        List<GameListItem> games = await RESTCalls.GetGamelist();
                        foreach (GameListItem game in games)
                        {
                            if (!LocalState.SupportedGames.ContainsKey(game.Id))
                                LocalState.SupportedGames.Add(game.Id, game);
                            if (LocalState.SupportedGamesNames.ContainsKey(game.Name))
                                LocalState.SupportedGamesNames.Add(game.Name, game.Id);
                        }
                    }

                }
                else
                {
                    App.CheckOnline();
                }
            }
            else
            {
                SubFrameNavigator(typeof(LogScreen));
            }
        }

        public class UserLogin
        {
            public string Token { get; set; }
            public string Name { get; set; }
            public SolidColorBrush Foreground { get; set; }
        }

        private void RefreshLoginList()
        {
            IReadOnlyList<PasswordCredential> tokens = Storage.PasswordVault.FindAllByResource("Token");
            List<UserLogin> users = new List<UserLogin>();
            foreach (PasswordCredential user in tokens)
            {
                UserLogin u = new UserLogin();
                u.Name = user.UserName;
                user.RetrievePassword();
                u.Token = user.Password;
            }
        }

        #endregion

        #region LogOut

        private void App_LogOutHandler(object sender, EventArgs e)
        {
            PasswordCredential creds;
            try
            {
                creds = Storage.PasswordVault.Retrieve("Token", LocalState.CurrentUser.Email);
            }
            catch
            {
                creds = Storage.PasswordVault.Retrieve("Token", "logintoken");
            }

            Storage.PasswordVault.Remove(creds);

            ClearData();

            SubFrameNavigator(typeof(LogScreen));
        }

        #endregion

        #region Navigation

        private async void App_NavigateToGuildHandler(object sender, App.GuildNavigationArgs e)
        {
            SubscribeToIndividualChannels = false;
            ServerWarnings.Children.Clear();
            App.SaveDraft();

            if (e.GuildId != App.CurrentGuildId)
            {
                memberscvs?.Clean();

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    (ServerList.SelectedItem as SimpleGuild).IsSelected = true;
                    MembersCvs.Source = null;
                });
            }

            App.CurrentGuildIsDM = e.GuildId == "@me"; //Could combine...
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (SimpleGuild guild in ServerList.Items)
                {
                    if (guild.Id == e.GuildId)
                    {
                        ServerList.SelectedItem = guild;
                        guild.IsSelected = true;
                    }
                    else
                    {
                        guild.IsSelected = false;
                    }
                }

                if (e.GuildId != "@me")
                {
                    if (UISize.CurrentState.Name != "ExtraLarge") MemberToggle.Visibility = Visibility.Visible;

                    App.CurrentGuildId = e.GuildId;
                    UserDetails.Visibility = Visibility.Collapsed;
                    MemberListFull.Visibility = Visibility.Visible;
                    CallUser.Visibility = Visibility.Collapsed;
                    RenderGuildChannels();
                    if (App.ShowAds) Ad.Visibility = Visibility.Visible;
                }
                else
                {
                    Ad.Visibility = Visibility.Collapsed;

                    App.CurrentGuildId = null;
                    MemberToggle.Visibility = Visibility.Collapsed;
                    //HideMemberToggle.Begin();
                    RenderDMChannels();
                }

                if (App.CurrentGuildId == null)
                {
                    string[] channels = new string[LocalState.DMs.Count];
                    for (int x = 0; x < LocalState.DMs.Count; x++)
                        channels[x] = LocalState.DMs.Values.ToList()[x].Id;
                    SubscribeToGuild(channels);
                }
                else
                {
                    string[] channels = new string[LocalState.Guilds[App.CurrentGuildId].channels.Count];
                    channels[0] = App.CurrentGuildId;
                    for (int x = 1; x < LocalState.Guilds[App.CurrentGuildId].channels.Count; x++)
                        channels[x] = LocalState.Guilds[App.CurrentGuildId].channels.Values.ToList()[x].raw.Id;
                    SubscribeToGuild(channels);
                }

                if (!App.ShowAds && Storage.Settings.SelectedChannels.ContainsKey(e.GuildId))
                {
                    foreach (SimpleChannel chn in channelCollection)
                        if (chn.Id == Storage.Settings.SelectedChannels[e.GuildId])
                        {
                            lastChangeProgrammatic = true;
                            ChannelList.SelectedItem = chn;
                        }

                    if (e.GuildId == "@me")
                        App.NavigateToDMChannel(Storage.Settings.SelectedChannels[e.GuildId]);
                    else
                        App.NavigateToGuildChannel(e.GuildId, Storage.Settings.SelectedChannels[e.GuildId]);
                }
            });
            App.UpdateUnreadIndicators();
        }

        private bool SubscribeToIndividualChannels;

        private async void SubscribeToGuild(string[] channels)
        {
            if (!await GatewayManager.Gateway.SubscribeToGuild(channels))
            {
                //Too many channels, they need to be subscribed to individually
                SubscribeToIndividualChannels = true;
                ServerWarnings.Children.Add(new MiniWarning("Typing indicators",
                    "This server has too many channels to show typing indicators in the channel list. You will however still see who is typing in the channel you are in."));
            }
            else
            {
                SubscribeToIndividualChannels = false;
            }
        }

        private void App_NavigateToGuildChannelHandler(object sender, App.GuildChannelNavigationArgs e)
        {
            if (App.CurrentGuildId == e.GuildId)
            {
                Ad.Visibility = Visibility.Collapsed;
                if (!e.OnBack) navigationHistory.Push(_currentPage);
                
                App.LastReadMsgId = LocalState.RPC.ContainsKey(e.ChannelId)
                    ? LocalState.RPC[e.ChannelId].LastMessageId
                    : null;
                RenderMessages();
                App.MarkChannelAsRead(e.ChannelId);
                MessageBody.Visibility = Visibility.Visible;
                _currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);

                if (e.OnBack)
                    foreach (SimpleChannel chn in channelCollection)
                        if (chn.Id == e.ChannelId)
                        {
                            lastChangeProgrammatic = true;
                            ChannelList.SelectedItem = chn;
                        }

                foreach (SimpleChannel chn in channelCollection)
                    if (chn.Id == e.ChannelId)
                        chn.IsSelected = true;
                    else if (chn.Type != 2)
                        chn.IsSelected = false;
                Storage.Settings.SelectedChannels[e.GuildId] = e.ChannelId;
            }
            else //Out of guild navigation
            {
                CallUser.Visibility = Visibility.Collapsed;
                if (!e.OnBack) navigationHistory.Push(_currentPage);

                foreach (SimpleGuild guild in ServerList.Items)
                    if (guild.Id == e.GuildId)
                    {
                        guild.IsSelected = true;
                        ServerList.SelectedItem = guild;
                    }
                    else
                    {
                        guild.IsSelected = false;
                    }

                RenderGuildChannels();
                foreach (SimpleChannel chn in channelCollection)
                    if (chn.Id == e.ChannelId)
                    {
                        chn.IsSelected = true;
                        ChannelList.SelectedItem = chn;
                    }
                    else if (chn.Type != 2)
                    {
                        chn.IsSelected = false;
                    }

                if (e.ChannelId != null) App.MarkChannelAsRead(e.ChannelId);
                if (LocalState.RPC.ContainsKey(e.ChannelId))
                    App.LastReadMsgId = LocalState.RPC[e.ChannelId].LastMessageId;
                _currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);
            }

            if (SubscribeToIndividualChannels) SubscribeToGuild(new[] {App.CurrentChannelId});
            UpdateTyping();
        }

        private void App_NavigateToDMChannelHandler(object sender, App.DMChannelNavigationArgs e)
        {
            _autoselectchannelcontent = null;

            if (!e.OnBack) navigationHistory.Push(_currentPage);

            if (App.CurrentGuildIsDM)
            {
                CallUser.Visibility = Visibility.Visible;

                if (e.ChannelId != null && LocalState.RPC.ContainsKey(e.ChannelId))
                    App.LastReadMsgId = LocalState.RPC[e.ChannelId].LastMessageId;
                else
                    App.LastReadMsgId = null;
                Storage.Settings.SelectedChannels["@me"] = e.ChannelId;
            }
            else
            {
                ServerList.SelectedIndex = 0;
                foreach (SimpleGuild guild in ServerList.Items)
                    if (guild.Id == "@me")
                    {
                        guild.IsSelected = true;
                        ChannelList.SelectedItem = guild;
                    }
                    else
                    {
                        guild.IsSelected = false;
                    }

                App.CurrentGuildIsDM = true;
                App.CurrentGuildId = null;
                if (e.ChannelId != null && LocalState.RPC.ContainsKey(e.ChannelId))
                    App.LastReadMsgId = LocalState.RPC[e.ChannelId].LastMessageId;
                else
                    App.LastReadMsgId = null;
                RenderDMChannels();
            }

            if (e.ChannelId != null)
            {
                if (App.CinematicMode) CinematicChannelName.Visibility = Visibility.Visible;
                if (LocalState.DMs[e.ChannelId].Type == 1)
                {
                    UserDetails.DisplayedMember = new GuildMember
                        {User = LocalState.DMs[e.ChannelId].Users.FirstOrDefault()};
                    UserDetails.Visibility = Visibility.Visible;
                    MemberListFull.Visibility = Visibility.Collapsed;
                }
                else
                {
                    RenderGroupMembers();
                    UserDetails.Visibility = Visibility.Collapsed;
                    MemberListFull.Visibility = Visibility.Visible;
                }

                MessageBody.Visibility = Visibility.Visible;
                App.MarkChannelAsRead(e.ChannelId);

                if (e.OnBack)
                    foreach (SimpleChannel chn in channelCollection)
                        if (chn.Id == e.ChannelId)
                        {
                            lastChangeProgrammatic = true;
                            ChannelList.SelectedItem = chn;
                        }

                foreach (SimpleChannel chn in channelCollection)
                    if (chn.Id == e.ChannelId)
                    {
                        chn.IsSelected = true;
                        ChannelList.SelectedItem = chn;
                    }
                    else
                    {
                        chn.IsSelected = false;
                    }

                UpdateTyping();

                RenderMessages();
            } else
            {
                OpenFriendPanel(null, null);
            }

            _currentPage = new Tuple<string, string>(App.CurrentGuildId, App.CurrentChannelId);
        }

        private async void App_ShowSubFrameHandler(object sender, App.ShowSubFrameEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 if (Storage.Settings.ExpensiveRender) content.Blur(2, 300).Start();
                 SubFrame.CacheSize = 0;

                 SubFrame.Navigate(e.page, e.args);
                 SubFrameMask.Fade(0.6f, 500, 0, 0).Start();
                 SubFrame.Visibility = Visibility.Visible;
                //SubFrame.IsFocusEngagementEnabled = true;
                SubFrame.Focus(FocusState.Keyboard);
                //SubFrame.IsFocusEngaged = true;
                //((Control)FocusManager.FindFirstFocusableElement(SubFrame)).Focus(FocusState.Keyboard);
            });
        }

        #endregion

        #region SubPages

        private async void SubFrameNavigator(Type page, object args = null)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (Storage.Settings.ExpensiveRender) content.Blur(2, 300).Start();
                    SubFrame.CacheSize = 0;

                    SubFrame.Navigate(page, args);
                    SubFrameMask.Fade(0.6f, 500, 0, 0).Start();
                    SubFrame.Visibility = Visibility.Visible;
                    //SubFrame.IsFocusEngagementEnabled = true;
                    SubFrame.Focus(FocusState.Keyboard);
                    //SubFrame.IsFocusEngaged = true;
                    //((Control)FocusManager.FindFirstFocusableElement(SubFrame)).Focus(FocusState.Keyboard);
                });
        }

        private async void App_SubpageClosedHandler(object sender, EventArgs e)
        {
            if (Storage.Settings.ExpensiveRender)
                content.Blur(0, 600).Start();
            else
                content.Blur(0, 0).Start();
            await SubFrameMask.Fade(0f, 300, 0, 0).StartAsync();
            SubFrame.IsFocusEngagementEnabled = false;
            SubFrame.IsFocusEngaged = false;
            if (App.FCU) //idk why but it's neccessary
                SubFrame.Content = null;
        }

        private void App_NavigateToBugReportHandler(object sender, App.BugReportNavigationArgs e)
        {
            SubFrameNavigator(typeof(BugReport), e.Exception);
        }

        private void App_NavigateToChannelEditHandler(object sender, App.ChannelEditNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.EditChannel), e.ChannelId);
        }

        private void App_NavigateToCreateBanHandler(object sender, App.CreateBanNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.GetString("/Dialogs/VerifyBan") +
                          LocalState.Guilds[App.CurrentGuildId].members[e.UserId].User.Username + "?",
                ConfirmMessage = App.GetString("/Dialogs/Ban"),
                SubMessage = "",
                StartText = "",
                PlaceHolderText = null,
                ConfirmRed = true,
                args = new Tuple<string, string, CreateGuildBan>(e.UserId, App.CurrentGuildId,
                    new CreateGuildBan {DeleteMessageDays = 0}),
                function = RESTCalls.CreateBan
            });
        }

        private void App_NavigateToCreateServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.GetString("/Dialogs/CreateServer"),
                StartText = "",
                PlaceHolderText = App.GetString("/Dialogs/ServerName"),
                SubMessage = App.GetString("/Dialogs/ServerGuidelinesDesc1") +
                             App.GetString("/Dialogs/ServerGuidelinesDesc2"),
                ConfirmMessage = App.GetString("/Dialogs/Create"),
                ConfirmRed = false,
                args = new List<object>(),
                function = RESTCalls.CreateGuild
            });
        }

        private void App_NavigateToDeleteChannelHandler(object sender, App.DeleteChannelNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.CurrentGuildIsDM
                    ? LocalState.DMs[e.ChannelId].Type == 1
                        ?
                        App.GetString("/Dialogs/CloseDMConfirm") +
                        LocalState.DMs[e.ChannelId].Users.FirstOrDefault().Username + "?"
                        : App.GetString("/Dialogs/LeaveGroup").Replace("<group>", LocalState.DMs[e.ChannelId].Name)
                    : App.GetString("/Dialogs/VerifyDelete") +
                      LocalState.Guilds[App.CurrentGuildId].channels[e.ChannelId].raw.Name + "?",
                SubMessage = "",
                StartText = "",
                PlaceHolderText = null,
                ConfirmMessage = App.GetString("/Dialogs/Leave"),
                ConfirmRed = true,
                args = e.ChannelId,
                function = RESTCalls.DeleteChannel
            });
        }

        private void App_NavigateToRemoveGroupUserHandler(object sender, App.RemoveGroupUserNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = e.UserId == LocalState.CurrentUser.Id
                    ? App.GetString("/Dialogs/LeaveGroup").Replace("<group>", LocalState.DMs[e.ChannelId].Name)
                    : App.GetString("/Dialogs/RemoveFromGroup")
                        .Replace("<user>",
                            LocalState.DMs[e.ChannelId].Users.FirstOrDefault(x => x.Id == e.UserId).Username)
                        .Replace("<group>", LocalState.DMs[e.ChannelId].Name),
                SubMessage = "",
                StartText = "",
                PlaceHolderText = null,
                ConfirmMessage = e.UserId == LocalState.CurrentUser.Id
                    ? App.GetString("/Dialogs/Leave")
                    : App.GetString("/Dialogs/Remove"),
                ConfirmRed = true,
                args = new Tuple<string, string>(e.ChannelId, e.UserId),
                function = RESTCalls.RemoveGroupUser
            });
        }

        private void App_NavigateToDeleteServerHandler(object sender, App.DeleteServerNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.GetString("/Dialogs/VerifyDelete") + LocalState.Guilds[e.GuildId].Raw.Name + "?",
                ConfirmMessage = App.GetString("/Dialogs/Delete"),
                SubMessage = "",
                StartText = "",
                PlaceHolderText = null,
                ConfirmRed = true,
                args = e.GuildId,
                function = RESTCalls.DeleteGuild
            });
        }

        private void App_NavigateToGuildEditHandler(object sender, App.GuildEditNavigationArgs e)
        {
            SubFrameNavigator(typeof(EditGuild), e.GuildId);
        }

        private void App_NavigateToJoinServerHandler(object sender, string e)
        {
            SubFrameNavigator(typeof(JoinServer), e);
        }

        private void App_NavigateToLeaveServerHandler(object sender, App.LeaverServerNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.GetString("/Dialogs/VerifyLeave") + LocalState.Guilds[e.GuildId].Raw.Name + "?",
                ConfirmMessage = App.GetString("/Dialogs/LeaveServer"),
                SubMessage = "",
                StartText = "",
                PlaceHolderText = null,
                ConfirmRed = true,
                args = e.GuildId,
                function = RESTCalls.LeaveServer
            });
        }

        private void App_NavigateToNicknameEditHandler(object sender, App.NicknameEditNavigationArgs e)
        {
            GuildMember member = LocalState.Guilds[App.CurrentGuildId].members[e.UserId];
            SubPageData pageData = new SubPageData
            {
                Message = App.GetString("/Dialogs/EditNickname"),
                ConfirmMessage = App.GetString("/Dialogs/Save"),
                SubMessage = "",
                StartText = member.Nick != null ? member.Nick : "",
                ConfirmRed = false,
                CanBeFancy = true,
                PlaceHolderText = member.User.Username,
                args = e.UserId == LocalState.CurrentUser.Id
                    ? new List<object> {App.CurrentGuildId}
                    : new List<object> {App.CurrentGuildId, e.UserId}
            };
            if (e.UserId == LocalState.CurrentUser.Id)
                pageData.function = RESTCalls.ModifyCurrentUserNickname;
            else
                pageData.function = RESTCalls.ModifyGuildMemberNickname;
            SubFrameNavigator(typeof(DynamicSubPage), pageData);
        }

        private void App_NavigateToProfileHandler(object sender, App.ProfileNavigationArgs e)
        {
            SubFrameNavigator(typeof(SubPages.UserProfile), e.User);
        }

        private void App_OpenAttachementHandler(object sender, Attachment e)
        {
            SubFrameNavigator(typeof(PreviewAttachement), e);
        }

        private void App_NavigateToChannelTopicHandler(object sender, App.ChannelTopicNavigationArgs e)
        {
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = e.Channel.Name,
                ConfirmMessage = "",
                SubMessage = e.Channel.Topic,
                StartText = "",
                PlaceHolderText = null,
                ConfirmRed = false,
                args = null,
                function = null
            });
        }

        private void App_NavigateToCreateChannelHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(CreateChannel));
        }

        private void App_NavigateToSettingsHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.Settings));
        }

        private void App_NavigateToAccountSettingsHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(SubPages.UserProfileCU));
        }

        private void App_NavigateToAboutHandler(object sender, bool e)
        {
            if (!e)
                SubFrameNavigator(typeof(About));
            else
                SubFrameNavigator(typeof(WhatsNew));
        }

        private void App_NavigateToAddServerHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(AddServer));
        }

        private void App_NavigateToMessageEditorHandler(object sender, App.MessageEditorNavigationArgs e)
        {
            SubFrameNavigator(typeof(ExtendedMessageEditor), e);
        }

        private void App_NavigateToIAPSHandler(object sender, EventArgs e)
        {
            SubFrameNavigator(typeof(InAppPurchases));
        }

        #endregion

        #region Flyouts

        private void App_MenuHandler(object sender, App.MenuArgs e)
        {
            e.Flyout.ShowAt(sender as UIElement, e.Point);
        }

        private void App_ShowMemberFlyoutHandler(object sender, App.ProfileNavigationArgs e)
        {
            if (!App.CurrentGuildIsDM)
            {
                if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(e.User.Id))
                    FlyoutManager
                        .MakeUserDetailsFlyout(LocalState.Guilds[App.CurrentGuildId].members[e.User.Id], e.WebHook)
                        .ShowAt(sender as FrameworkElement);
                else
                    FlyoutManager.MakeUserDetailsFlyout(e.User, e.WebHook).ShowAt(sender as FrameworkElement);
            }
            else
            {
                FlyoutManager.MakeUserDetailsFlyout(e.User, e.WebHook).ShowAt(sender as FrameworkElement);
            }
        }

        private void App_ShowGameFlyoutHandler(object sender, string e)
        {
            FlyoutManager.MakeGameFlyout(e).ShowAt(sender as FrameworkElement);
        }

        #endregion

        #region Link

        private async void App_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.Link.StartsWith("#"))
            {
                string val = e.Link.Remove(0, 1);
                App.NavigateToGuildChannel(App.CurrentGuildId, val);
            }
            else if (e.Link.StartsWith("@!"))
            {
                string val = e.Link.Remove(0, 2);
                if (App.CurrentGuildId == null)
                {
                    if (e.User != null)
                        App.ShowMemberFlyout(sender, e.User, false);
                }
                else if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(val))
                {
                    App.ShowMemberFlyout(sender, LocalState.Guilds[App.CurrentGuildId].members[val].User, false);
                }
                else if (e.User != null)
                {
                    App.ShowMemberFlyout(sender, e.User, false);
                }
            }
            else if (e.Link.StartsWith("@&"))
            {
                string val = e.Link.Remove(0, 2);
                foreach (Grouping<HoistRole, Member> group in memberscvs)
                    if (group.Group.Id == val)
                    {
                        sideDrawer.OpenRight();
                        MembersListView.ScrollIntoView(group);
                        return;
                    }
            }
            else if (e.Link.StartsWith("@"))
            {
                string val = e.Link.Remove(0, 1);
                if (App.CurrentGuildIsDM)
                {
                    App.ShowMemberFlyout(sender,
                        LocalState.DMs[App.CurrentChannelId].Users.FirstOrDefault(x => x.Id == val), false);
                }
                else
                {
                    if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(val))
                        App.ShowMemberFlyout(sender, LocalState.Guilds[App.CurrentGuildId].members[val].User, false);
                    else if (e.User != null)
                        App.ShowMemberFlyout(sender, e.User, false);
                    //App.ShowMemberFlyout(sender, val);
                }
            }
            else if (e.Link.StartsWith("https://discordapp.com/channels"))
            {
                string[] segments = e.Link.Substring(32).Split('/');
                App.NavigateToGuildChannel(segments[0], segments[1]);
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }

        #endregion

        #region API



        //The typing cooldown disables the trigger typing event from being fired if it was already triggered less than 5 seconds ago
        //This is to avoid 429 errors (too many requests) because otherwise it would fire on every letter
        //This also improves performance
        private readonly DispatcherTimer typingCooldown = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};

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

        private async void App_AddFriendHandler(object sender, App.AddFriendArgs e)
        {
            await RESTCalls.SendFriendRequest(e.UserId);
        }

        private async void App_BlockUserHandler(object sender, App.BlockUserArgs e)
        {
            await RESTCalls.BlockUser(e.UserId);
        }

        private async void App_MarkMessageAsReadHandler(object sender, App.MarkMessageAsReadArgs e)
        {
            await RESTCalls.AckMessage(e.ChannelId, e.MessageId);
        }

        private void App_MarkCategoryAsReadHandler(object sender, App.MarkCategoryAsReadArgs e)
        {
            foreach (var channel in LocalState.Guilds[e.GuildId].channels.Values)
            {
                if (channel.raw.ParentId == e.ChannelId)
                {
                    App.MarkChannelAsRead(channel.raw.Id);
                }
            }
        }

        private async void App_MarkChannelAsReadHandler(object sender, App.MarkChannelAsReadArgs e)
        {
            //Assumes you marked it from active guild
            if (!App.CurrentGuildIsDM)
            {
                if (LocalState.Guilds[App.CurrentGuildId].channels.ContainsKey(e.ChannelId))
                    if (LocalState.Guilds.ContainsKey(App.CurrentGuildId) &&
                        LocalState.Guilds[App.CurrentGuildId].channels.ContainsKey(e.ChannelId))
                        await RESTCalls.AckMessage(e.ChannelId,
                            LocalState.Guilds[App.CurrentGuildId].channels[e.ChannelId].raw.LastMessageId);
            }
            else
            {
                if (LocalState.DMs.ContainsKey(e.ChannelId))
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
            Dictionary<string, ChannelOverride> chns = new Dictionary<string, ChannelOverride>();
            ChannelOverride chan;
            if (!LocalState.GuildSettings.ContainsKey(App.CurrentGuildId))
                LocalState.GuildSettings.Add(App.CurrentGuildId,
                    new LocalModels.GuildSetting(new GuildSetting {GuildId = App.CurrentGuildId}));

            if (!LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.ContainsKey(e.ChannelId))
                LocalState.GuildSettings[App.CurrentGuildId].channelOverrides.Add(e.ChannelId,
                    new ChannelOverride {Channel_Id = e.ChannelId, MessageNotifications = 0, Muted = true});

            chan = LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[e.ChannelId];
            chan.Channel_Id = null;
            chan.Muted = !chan.Muted;
            chns.Add(e.ChannelId, chan);

            GuildSetting returned = await RESTCalls.ModifyGuildSettings(App.CurrentGuildId,
                new GuildSettingModify {ChannelOverrides = chns});

            LocalState.GuildSettings[App.CurrentGuildId].raw = returned;

            foreach (ChannelOverride chn in returned.ChannelOverrides)
                if (chn.Channel_Id == e.ChannelId)
                    LocalState.GuildSettings[App.CurrentGuildId].channelOverrides[e.ChannelId] = chn;
            App.UpdateUnreadIndicators();
        }

        private async void App_MuteGuildHandler(object sender, App.MuteGuildArgs e)
        {
            if (!LocalState.GuildSettings.ContainsKey(e.GuildId))
                LocalState.GuildSettings.Add(e.GuildId,
                    new LocalModels.GuildSetting(
                        await RESTCalls.ModifyGuildSettings(e.GuildId, new GuildSettingModify {Muted = true})));
            else
                LocalState.GuildSettings[e.GuildId] = new LocalModels.GuildSetting(
                    await RESTCalls.ModifyGuildSettings(e.GuildId,
                        new GuildSettingModify {Muted = !LocalState.GuildSettings[e.GuildId].raw.Muted}));
            App.UpdateUnreadIndicators();
        }

        private async void App_RemoveFriendHandler(object sender, App.RemoveFriendArgs e)
        {
            await RESTCalls.RemoveFriend(e.UserId);
        }

        private async void App_UpdatePresenceHandler(object sender, App.UpdatePresenceArgs e)
        {
            await RESTCalls.ChangeUserStatus(e.Status);
        }

        private async void App_VoiceConnectHandler(object sender, App.VoiceConnectArgs e)
        {
            if (e.ChannelId != null)
                VoiceController.Show();
            else
            {
                VoiceController.Hide();
                if (Storage.Settings.BackgroundVoice)
                {
                    VoiceManager.voipCall.NotifyCallEnded();
                }
            }

            foreach (SimpleChannel chn in channelCollection)
                if (chn.Type == 2)
                {
                    if (e.ChannelId == chn.Id)
                        chn.IsSelected = true;
                    else
                        chn.IsSelected = false;
                }

            await GatewayManager.Gateway.VoiceStatusUpdate(e.GuildId, e.ChannelId, LocalState.VoiceState.SelfMute,
                LocalState.VoiceState.SelfDeaf);
        }

        #endregion

        #region UpdateUI

        #region RenderElement


        public void RenderCurrentUser()
        {
            ImageBrush image = new ImageBrush
            {
                ImageSource =
                    new BitmapImage(Common.AvatarUri(LocalState.CurrentUser.Avatar, LocalState.CurrentUser.Id))
            };

            if (LocalState.CurrentUser.Avatar == null)
                AvatarBG.Fill = Common.DiscriminatorColor(LocalState.CurrentUser.Discriminator);
            else
                AvatarBG.Fill = Common.GetSolidColorBrush(Colors.Transparent);

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
            SimpleGuild DM = new SimpleGuild();
            DM.Id = "@me";
            DM.Name = App.GetString("/Main/DirectMessages");
            DM.IsDM = false;
            foreach (DirectMessageChannel chn in LocalState.DMs.Values)
                if (LocalState.RPC.ContainsKey(chn.Id))
                {
                    ReadState readstate = LocalState.RPC[chn.Id];
                    DM.NotificationCount += readstate.MentionCount;
                    DirectMessageChannel StorageChannel = LocalState.DMs[chn.Id];
                    if (StorageChannel.LastMessageId != null &&
                        readstate.LastMessageId != StorageChannel.LastMessageId)
                        DM.IsUnread = true;
                }

            ServerList.Items.Add(DM);

            foreach (KeyValuePair<string, LocalModels.Guild> guild in LocalState.Guilds.OrderBy(x => x.Value.Position))
            {
                SimpleGuild sg = new SimpleGuild();
                sg.Id = guild.Value.Raw.Id;
                if (!string.IsNullOrEmpty(guild.Value.Raw.Icon))
                    sg.ImageURL = "https://cdn.discordapp.com/icons/" + guild.Value.Raw.Id + "/" +
                                  guild.Value.Raw.Icon + ".png";
                else
                    sg.ImageURL = "empty";
                sg.Name = guild.Value.Raw.Name;

                sg.IsMuted = LocalState.GuildSettings.ContainsKey(guild.Key) &&
                             LocalState.GuildSettings[guild.Key].raw.Muted;
                sg.IsUnread = false; //Will change if true
                foreach (LocalModels.GuildChannel chn in guild.Value.channels.Values)
                    if (LocalState.RPC.ContainsKey(chn.raw.Id))
                    {
                        ReadState readstate = LocalState.RPC[chn.raw.Id];
                        sg.NotificationCount += readstate.MentionCount;
                        DiscordAPI.SharedModels.GuildChannel storageChannel = LocalState.Guilds[sg.Id].channels[chn.raw.Id].raw;
                        if (readstate.LastMessageId != storageChannel.LastMessageId && !sg.IsMuted)
                            sg.IsUnread = true;
                    }

                sg.IsValid = guild.Value.valid;
                ServerList.Items.Add(sg);
            }
        }
        
        public void RenderDMChannels(string id = null)
        {
            friendPanel.Visibility = Visibility.Collapsed;
            PinnedMessags.Visibility = Visibility.Collapsed;
            MessageBody.Visibility = Visibility.Collapsed;

            ChannelLoading.IsActive = true;
            ChannelLoading.Visibility = Visibility.Visible;
            //if (App.CinematicMode)
            //    CineGuildNameBTN.Visibility = Visibility.Collapsed;
            //else
                ServerNameButton.Visibility = Visibility.Collapsed;

            FriendsItem.Visibility = Visibility.Visible;
            DirectMessageBlock.Visibility = Visibility.Visible;

            //Select FriendPanel
            if (id == null)
            {
                App.CurrentChannelId = null;
                FriendsItem.IsSelected = true;
                friendPanel.Visibility = Visibility.Visible;
                //MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
                CallUser.Visibility = Visibility.Collapsed;
                if (App.CinematicMode) CinematicChannelName.Visibility = Visibility.Collapsed;
            }

            AddChannelFlyoutSep.Visibility = Visibility.Collapsed;
            AddChannelFlyoutItem.Visibility = Visibility.Collapsed;

            ChannelName.Text =  ChannelTopic.Text = CinematicChannelName.Text = "";

            channelCollection.Clear();

            foreach (SimpleChannel channel in ChannelManager.OrderChannels(LocalState.DMs.Values.ToList()))
                if (App.CurrentGuildIsDM)
                {
                    channelCollection.Add(channel);
                    if (id != null && channel.Id == id)
                    {
                        CallUser.Visibility = Visibility.Collapsed;
                        ChannelList.SelectedItem = channel;
                        App.CurrentChannelId = id;
                    }

                    if (!string.IsNullOrEmpty(_autoselectchannel))
                        if (channel.Id == _autoselectchannel)
                        {
                            ChannelSelectionWasClicked = true; //hehe, not actually true
                            ChannelList.SelectedItem = channel;
                        }
                }

            ChannelLoading.IsActive = false;
            ChannelLoading.Visibility = Visibility.Collapsed;
        }

        public void RenderGuildChannels() //App.CurrentGuildId is set
        {
            MessageBody.Visibility = Visibility.Collapsed;
            friendPanel.Visibility = Visibility.Collapsed;
            PinnedMessags.Visibility = Visibility.Collapsed;

            ChannelLoading.IsActive = true;
            ChannelLoading.Visibility = Visibility.Visible;
            //if (App.CinematicMode)
            //    CineGuildNameBTN.Visibility = Visibility.Visible;
            //else
                ServerNameButton.Visibility = Visibility.Visible;
            FriendsItem.Visibility = Visibility.Collapsed;
            DirectMessageBlock.Visibility = Visibility.Collapsed;

            if (LocalState.CurrentGuild.permissions.CreateInstantInvite)
            {
                CreateInviteFlyoutItem.Visibility = Visibility.Visible;
                CreateInviteFlyoutSep.Visibility = Visibility.Visible;
            }
            else
            {
                CreateInviteFlyoutItem.Visibility = Visibility.Collapsed;
                CreateInviteFlyoutSep.Visibility = Visibility.Collapsed;
            }

            if (LocalState.CurrentGuild.permissions.ViewAuditLog)
            {
                AuditLogFlyoutItem.Visibility = Visibility.Visible;
            }
            else
            {
                AuditLogFlyoutItem.Visibility = Visibility.Collapsed;
            }

            if (LocalState.CurrentGuild.permissions.ManageChannels)
            {
                AddChannelFlyoutSep.Visibility = Visibility.Visible;
                AddChannelFlyoutItem.Visibility = Visibility.Visible;
            }
            else
            {
                AddChannelFlyoutSep.Visibility = Visibility.Collapsed;
                AddChannelFlyoutItem.Visibility = Visibility.Collapsed;
            }

            if (LocalState.CurrentGuild.Raw.OwnerId == LocalState.CurrentUser.Id)
            {
                LeaveGuildFlyoutItem.Text = App.GetString("/Main/DeleteServerMFI");
            } else
            {
                LeaveGuildFlyoutItem.Text = App.GetString("/Main/LeaveServerMFI");
            }

            ChannelName.Text = ChannelTopic.Text = CinematicChannelName.Text = "";

            ServerName.Text = LocalState.Guilds[App.CurrentGuildId].Raw.Name;

            channelCollection.Clear();

            foreach (SimpleChannel channel in ChannelManager.OrderChannels(LocalState.Guilds[App.CurrentGuildId]
                .channels.Values.ToList()))
            {
                if (VoiceController.channelid == channel.Id)
                    channel.IsSelected = true;
                channelCollection.Add(channel);
                if (!string.IsNullOrEmpty(_autoselectchannel))
                    if (channel.Id == _autoselectchannel)
                    {
                        ChannelSelectionWasClicked = true; //hehe, not actually true
                        ChannelList.SelectedItem = channel;
                    }
            }

            ChannelLoading.IsActive = false;
            ChannelLoading.Visibility = Visibility.Collapsed;
        }


        //bool MessageRange_LastMessage = false;
        public async void RenderMessages() //App.CurrentChannelId is set
        {
            FriendsItem.IsSelected = false;
            friendPanel.Visibility = Visibility.Collapsed;
            PinnedMessags.Visibility = Visibility.Visible;

            if (App.CinematicMode) CinematicChannelName.Visibility = Visibility.Visible;
            if (UISize.CurrentState == Small) sideDrawer.CloseLeft();
            if (ChannelList.SelectedItem != null)
            {
                ChannelName.Text = CinematicChannelName.Text = (ChannelList.SelectedItem as SimpleChannel).Type == 0
                    ? "#" + (ChannelList.SelectedItem as SimpleChannel)?.Name
                    : (ChannelList.SelectedItem as SimpleChannel)?.Name;
                //CompChannelName.Text = ChannelName.Text;
                ChannelTopic.Text = (ChannelList.SelectedItem as SimpleChannel).Type == 0
                    ? LocalState.Guilds[App.CurrentGuildId].channels[(ChannelList.SelectedItem as SimpleChannel)?.Id]
                        .raw
                        .Topic
                    : "";
            }

            //CompChannelTopic.Text = ChannelTopic.Text;
            if (ChannelTopic.Text == null || ChannelTopic.Text.Trim() == "")
            {
                ChannelTopic.Visibility = Visibility.Collapsed;
                ChannelName.Margin = new Thickness(0, 10, 0, 0);
            }
            else
            {
                ChannelTopic.Visibility = Visibility.Visible;
                ChannelName.Margin = new Thickness(0, 0, 0, 0);
            }

            IEnumerable<Message> epinnedmessages = null;
            await Task.Run(async () =>
            {
                epinnedmessages = await RESTCalls.GetChannelPinnedMessages(App.CurrentChannelId);
            });
            if (epinnedmessages != null)
            {
                List<MessageContainer> pinnedmessages = await MessageManager.ConvertMessage(epinnedmessages.ToList());
                pinnedmessages.Reverse();
                if (pinnedmessages != null)
                    foreach (MessageContainer message in pinnedmessages)
                        PinnedMessageList.Items.Add(message);
            }

            if (PinnedMessageList.Items.Count == 0)
                NoPinnedMessages.Visibility = Visibility.Visible;
            else
                NoPinnedMessages.Visibility = Visibility.Collapsed;
            sideDrawer.CloseLeft();
        }

        public void RenderGroupMembers()
        {
            if (memberscvs != null)
                memberscvs.Clean();
            List<Member> tempMembers = new List<Member>();
            foreach (DiscordAPI.SharedModels.User user in LocalState.DMs[App.CurrentChannelId].Users)
            {
                Member m = new Member(new GuildMember
                {
                    User = user
                });
                m.DisplayName = user.Username;
                m.MemberHoistRole = new HoistRole("MEMBERS", 0, "MEMBERS", 0, -1);
                if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                    m.status = LocalState.PresenceDict[m.Raw.User.Id];
                else
                    m.status = new Presence {Status = "offline", Game = null};
                tempMembers.Add(m);
            }

            Member cm = new Member(new GuildMember
            {
                User = LocalState.CurrentUser
            });
            cm.DisplayName = LocalState.CurrentUser.Username;
            cm.MemberHoistRole = new HoistRole("MEMBERS", 0, "MEMBERS", 0, -1);
            if (LocalState.PresenceDict.ContainsKey(cm.Raw.User.Id))
                cm.status = LocalState.PresenceDict[cm.Raw.User.Id];
            else
                cm.status = new Presence {Status = "offline", Game = null};
            tempMembers.Add(cm);

            memberscvs = new GroupedObservableCollection<HoistRole, Member>(c => c.MemberHoistRole, tempMembers);

            MembersCvs.Source = memberscvs;
            //  memberscvs.Clear();
            //MembersCVS.Source = memberscvs.SkipWhile(m => m.Value.status.Status == "offline").GroupBy(m => m.Value.MemberDisplayedRole).OrderBy(m => m.Key.Position).ToList();
            //MembersCvs.Source = memberscvs.OrderBy(m => m.Value.Raw.User.Username).GroupBy(m => m.Value.status);
        }

        public void UpdateTyping()
        {
            foreach (SimpleChannel channel in channelCollection)
                channel.IsTyping = LocalState.Typers.ContainsKey(channel.Id);
        }

        private readonly int TempGuildCount = 0;
        private List<SimpleGuild> oldTempGuilds;

        private async void UpdateGuildAndChannelUnread()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    FriendNotificationCounter.Text = App.FriendNotifications.ToString();
                    if (FriendNotificationCounter.Text != "0")
                        ShowFriendsBadge.Begin();
                    else
                        HideFriendsBadge.Begin();
                    int Fullcount = 0;

                    foreach (SimpleGuild guild in ServerList.Items)
                    {
                        SimpleGuild gclone = guild.Clone();
                        gclone.NotificationCount = 0; //Will Change if true
                        gclone.IsUnread = false; //Will change if true
                        if (gclone.Id == "@me")
                        {
                            if (App.FriendNotifications > 0 && Storage.Settings.FriendsNotifyFriendRequest)
                                gclone.NotificationCount += App.FriendNotifications;
                            List<SimpleGuild> TempGuilds = new List<SimpleGuild>();
                            foreach (DirectMessageChannel chn in LocalState.DMs.Values)
                                if (LocalState.RPC.ContainsKey(chn.Id))
                                {
                                    ReadState readstate = LocalState.RPC[chn.Id];
                                    if (Storage.Settings.FriendsNotifyDMs)
                                    {
                                        gclone.NotificationCount += readstate.MentionCount;
                                        Fullcount += readstate.MentionCount;
                                    }

                                    // var StorageChannel = LocalState.DMs[chn.Id];
                                    //  if (StorageChannel.LastMessageId != null && readstate.LastMessageId != StorageChannel.LastMessageId)
                                    //     gclone.NotificationCount += 1;
                                    /*    if (readstate.MentionCount > 0)
                                        {
                                            SimpleGuild tempguild = new SimpleGuild()
                                            {
                                                Name = chn.Name,
                                                IsDM = true,
                                                NotificationCount = readstate.MentionCount,
                                                Id = chn.Id,
                                                TempLastMessageId = chn.LastMessageId
                                            };
                                            TempGuilds.Add(tempguild);
                                        }*/
                                }

                            //Remove all TempGuilds from serverlist;
                            /*bool TempGuildZone = true;
                            while (TempGuildZone)
                            {
                                if (((SimpleGuild)ServerList.Items[1]).IsDM)
                                    ServerList.Items.RemoveAt(1);
                                else
                                    TempGuildZone = false;
                            }
                            //Add all tempguilds
                            foreach (var tempguild in TempGuilds.OrderBy(x => Common.SnowflakeToTime(x.TempLastMessageId)).Reverse())
                                ServerList.Items.Insert(1, tempguild);
                            TempGuildCount = TempGuilds.Count;*/
                        }
                        else if (!guild.IsDM)
                        {
                            if (LocalState.GuildSettings.ContainsKey(gclone.Id))
                                gclone.IsMuted = LocalState.GuildSettings[gclone.Id].raw.Muted;
                            else
                                gclone.IsMuted = false;
                            ICollection<string> channelkeys = LocalState.Guilds[gclone.Id].channels.Keys;
                            int keycount = channelkeys.Count;
                            foreach (string key in channelkeys)
                            {
                                LocalModels.GuildChannel chn = LocalState.Guilds[gclone.Id].channels[key];
                                if (LocalState.RPC.ContainsKey(chn.raw.Id))
                                {
                                    LocalModels.GuildChannel chan = LocalState.Guilds[gclone.Id].channels[chn.raw.Id];
                                    ReadState readstate = LocalState.RPC[chn.raw.Id];

                                    bool Muted = LocalState.GuildSettings.ContainsKey(gclone.Id)
                                        ? LocalState.GuildSettings[gclone.Id].channelOverrides.ContainsKey(chan.raw.Id)
                                            ? LocalState.GuildSettings[gclone.Id].channelOverrides[chan.raw.Id].Muted
                                            : false
                                        : false;

                                    gclone.NotificationCount += readstate.MentionCount;
                                    Fullcount += readstate.MentionCount;

                                    if (chan.raw.LastMessageId != null
                                        && chan.raw.LastMessageId != readstate.LastMessageId &&
                                        (Storage.Settings.mutedChnEffectServer || !Muted)
                                    ) //if channel is unread and not muted
                                        gclone.IsUnread = true;
                                }
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
                        foreach (SimpleChannel sc in channelCollection)
                        {
                            if (LocalState.RPC.ContainsKey(sc.Id))
                            {
                                ReadState readstate = LocalState.RPC[sc.Id];
                                sc.NotificationCount = readstate.MentionCount;
                                //Just ignore unread indicators for DMs:

                                //  var StorageChannel = LocalState.DMs[sc.Id];
                                // if (StorageChannel.LastMessageId != null &&
                                //   readstate.LastMessageId != StorageChannel.LastMessageId)
                                //  sc.IsUnread = true;
                                //  else
                                //    sc.IsUnread = false;
                            }

                            sc.IsUnread = false;
                        }
                    }
                    else
                    {
                        if (App.CurrentGuildId != null) //Incase called before intiialization
                            foreach (SimpleChannel sc in channelCollection)
                                if (LocalState.RPC.ContainsKey(sc.Id) &&
                                    LocalState.Guilds[App.CurrentGuildId].channels.ContainsKey(sc.Id))
                                {
                                    ReadState readstate = LocalState.RPC[sc.Id];
                                    sc.NotificationCount = readstate.MentionCount;
                                    LocalModels.GuildChannel storageChannel = LocalState.Guilds[App.CurrentGuildId].channels[sc.Id];
                                    if (storageChannel != null && storageChannel.raw.LastMessageId != null &&
                                        readstate.LastMessageId != storageChannel.raw.LastMessageId)
                                        sc.IsUnread = true;
                                    else
                                        sc.IsUnread = false;

                                    sc.IsMuted = LocalState.GuildSettings.ContainsKey(App.CurrentGuildId) &&
                                                 LocalState.GuildSettings[App.CurrentGuildId].channelOverrides
                                                     .ContainsKey(sc.Id) && LocalState.GuildSettings[App.CurrentGuildId]
                                                     .channelOverrides[sc.Id].Muted;
                                }
                    }

                    if (Storage.Settings.FriendsNotifyFriendRequest) Fullcount += App.FriendNotifications;

                    if (App.FriendNotifications > 0)
                    {
                        FriendNotificationCounter.Text = App.FriendNotifications.ToString();
                        ShowFriendsBadge.Begin();
                    }
                    else
                    {
                        HideFriendsBadge.Begin();
                    }

                    if (Fullcount != App.AllNotifications)
                    {
                        if (Fullcount > 0)
                            App.AllNotifications = Fullcount;
                        else
                            App.AllNotifications = Fullcount;

                        if (Storage.Settings.Badge)
                        {
                            int count = 0;
                            foreach (ReadState chn in LocalState.RPC.Values.ToList()) count += chn.MentionCount;
                            NotificationManager.SendBadgeNotification(count);
                        }
                    }

                    RefreshVisibilityIndicators();
                });
        }
        #endregion

        private void SetupUI()
        {
            //Remove clipping from all the listviews
            Common.RemoveScrollviewerClipping(ServerScrollviewer);
            Common.RemoveScrollviewerClipping(ChannelScrollviewer);
            Common.RemoveScrollviewerClipping(MembersListView);
        }

        private async void StartAppService()
        {
            //Permission not set
            if (App.IsDesktop)
            {
                //await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private async void App_ReadyRecievedHandler(object sender, EventArgs e)
        {
            loadingStack.Loading("Finished", "CONNECTED");
            loadingStack.Loaded("GatewayConnecting");
            GatewayManager.Gateway.GuildMemberRemoved += Gateway_GuildMemberRemoved;
            GatewayManager.Gateway.GuildMemberAdded += Gateway_GuildMemberAdded;
            GatewayManager.Gateway.GuildMemberUpdated += Gateway_GuildMemberUpdated;
            GatewayManager.Gateway.ChannelRecipientAdded += Gateway_ChannelRecipientAdded;
            GatewayManager.Gateway.ChannelRecipientRemoved += Gateway_ChannelRecipientRemoved;

            if (App.Insider && App.IsDesktop) StartAppService();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                friendPanel.Load();
                DisconnectedMask.Visibility = Visibility.Collapsed;
                SetupUI();
                RenderCurrentUser();
                RenderGuilds();
                ServerList.SelectedIndex = 0;

                UserStatusIndicator.Fill =
                    (SolidColorBrush) Application.Current.Resources[
                        LocalState.CurrentUserPresence.Status];
                switch (LocalState.CurrentUserPresence.Status)
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
                    case "offline":
                        UserStatusInvisible.IsChecked = true;
                        break;
                }

                App.UpdateUnreadIndicators();
                App.FullyLoaded = true;
                if (App.PostLoadTask != null)
                    switch (App.PostLoadTask)
                    {
                        case "SelectGuildChannelTask":
                            App.SelectGuildChannel(((App.GuildChannelSelectArgs) App.PostLoadTaskArgs).GuildId,
                                ((App.GuildChannelSelectArgs) App.PostLoadTaskArgs).ChannelId);
                            break;
                        case "SelectDMChannelTask":
                            App.SelectDMChannel((App.DMChannelSelectArgs) App.PostLoadTaskArgs);
                            break;
                        case "invite":
                            App.NavigateToJoinServer(((App.GuildChannelSelectArgs) App.PostLoadTaskArgs).GuildId);
                            break;
                    }
                //Check version number, and if it's different from before, open the what's new page
                Package package = Package.Current;
                PackageId packageId = package.Id;
                string version = packageId.Version.Build + packageId.Version.Major.ToString() +
                                 packageId.Version.Minor;

                if (Microsoft.Toolkit.Uwp.Helpers.SystemInformation.IsAppUpdated)
                {
                    App.NavigateToAbout(true);
                }

                loadingStack.Loaded("Finished");
                //if (Storage.Settings.VideoAd)
                //{
                //    InterstitialAd videoAd = new InterstitialAd();
                //    videoAd.AdReady += VideoAd_AdReady;
                //    videoAd.ErrorOccurred += VideoAd_ErrorOccurred;
                //    videoAd.RequestAd(AdType.Video, "9nbrwj777c8r", "1100015338");
                //}
            });
            if (_setupArgs != "")
            {
                if (_setupArgs.StartsWith("quarrel://"))
                {
                    string[] segments = _setupArgs.Replace("quarrel://", "").Split('/');
                    int count = segments.Count();
                    if (count > 0)
                    {
                        if (segments[0] == "guild")
                        {
                            if (count == 3)
                                App.SelectGuildChannel(segments[1], segments[2]);
                            else if (count == 2)
                                App.SelectGuildChannel(segments[1], null);
                        }
                        else if (segments[0] == "invite")
                        {
                            App.NavigateToJoinServer(segments[1]);
                        }
                    }
                }
                else if (_setupArgs == "SHARETARGET")
                {
                    SubFrameNavigator(typeof(ExtendedMessageEditor));
                    navigationHistory.Clear();
                }
            }
        }

        private async void Gateway_ChannelRecipientRemoved(object sender, GatewayEventArgs<ChannelRecipientUpdate> e)
        {
            if (LocalState.DMs.ContainsKey(e.EventData.channel_id) && LocalState.DMs[e.EventData.channel_id].Users
                    .FirstOrDefault(x => x.Id == e.EventData.user.Id) != null)
                LocalState.DMs[e.EventData.channel_id].Users.Remove(LocalState.DMs[e.EventData.channel_id].Users
                    .FirstOrDefault(x => x.Id == e.EventData.user.Id));
            if (App.CurrentGuildIsDM)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (SimpleChannel sc in channelCollection)
                        if (sc.Id == e.EventData.channel_id && LocalState.DMs.ContainsKey(e.EventData.channel_id))
                            sc.Subtitle = App.GetString("/Main/members").Replace("<count>",
                                (LocalState.DMs[e.EventData.channel_id].Users.Count() + 1).ToString());
                });

            if (App.CurrentChannelId == e.EventData.channel_id || !App.CurrentGuildIsDM)
                if (App.CurrentChannelId == e.EventData.channel_id)
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () => { memberscvs.Remove(FindMember(e.EventData.user.Id)); });
        }


        private async void Gateway_ChannelRecipientAdded(object sender, GatewayEventArgs<ChannelRecipientUpdate> e)
        {
            if (LocalState.DMs.ContainsKey(e.EventData.channel_id))
                LocalState.DMs[e.EventData.channel_id].Users.Add(e.EventData.user);
            if (App.CurrentGuildIsDM)
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (SimpleChannel sc in channelCollection)
                        if (sc.Id == e.EventData.channel_id && LocalState.DMs.ContainsKey(e.EventData.channel_id))

                            sc.Subtitle = App.GetString("/Main/members").Replace("<count>",
                                (LocalState.DMs[e.EventData.channel_id].Users.Count() + 1).ToString());
                });
            if (App.CurrentChannelId == e.EventData.channel_id)
            {
                Member m = new Member(new GuildMember
                {
                    User = e.EventData.user
                });
                AddToMembersCvs(m, true);
            }
        }

        private void VideoAd_AdReady(object sender, object e)
        {
            (sender as InterstitialAd).Show();
        }

        private async void VideoAd_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            //Storage.Settings.VideoAd = false;
            Storage.SettingsChanged();
            Storage.SaveAppSettings();

            MessageDialog msg = new MessageDialog("Couldn't find a video ad to show, showing banner ads");
            await msg.ShowAsync();
        }

        private async void App_TypingHandler(object sender, App.TypingArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                UpdateTyping);
        }

        private async void App_UpdateUnreadIndicatorsHandler(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                UpdateGuildAndChannelUnread);
        }

        private async void App_UserStatusChangedHandler(object sender, App.UserStatusChangedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (e.Settings.GuildOrder != null)
                    {
                        int position = 1;
                        foreach (string guild in e.Settings.GuildOrder)
                        {
                            object item = ServerList.Items.FirstOrDefault(x => (x as SimpleGuild).Id == guild);
                            if (item == null) return;
                            if (ServerList.Items.IndexOf(item) != position)
                            {
                                ServerList.Items.Remove(item);
                                ServerList.Items.Insert(position + TempGuildCount, item);
                            }

                            position++;
                        }

                        position = 0;
                    }

                    if (e.Settings.Status != null)
                    {
                        if (e.Settings.Status != "invisible")
                            UserStatusIndicator.Fill =
                                (SolidColorBrush) Application.Current.Resources[e.Settings.Status];
                        else
                            UserStatusIndicator.Fill = (SolidColorBrush) Application.Current.Resources["offline"];
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
                            case "offline":
                                UserStatusInvisible.IsChecked = true;
                                break;
                        }

                        Member member = FindMember(LocalState.CurrentUser.Id);
                        if (member == null) return;
                        member.status = new Presence
                        {
                            Game = member.status.Game, GuildId = member.status.GuildId, Roles = member.status.Roles,
                            Status = e.Settings.Status, User = member.status.User
                        };
                        if (LocalState.PresenceDict.ContainsKey(LocalState.CurrentUser.Id))
                            LocalState.PresenceDict[LocalState.CurrentUser.Id] = member.status;
                    }
                });
        }
        
        #region DMs

        private async void App_DMCreatedHandler(object sender, App.DMCreatedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (channelCollection.Count > 0)
                    {
                        SimpleChannel chn = ChannelManager.MakeChannel(e.DMChannel);
                        if (chn != null)
                            channelCollection.Insert(findLocation(chn), chn);
                    }
                });
        }

        private async void App_DMDeletedHandler(object sender, App.DMDeletedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    for (int i = 0; i < channelCollection.Count; i++)
                        if (channelCollection[i].Id == e.DMId)
                        {
                            channelCollection.RemoveAt(i);
                            break;
                        }
                });
        }

        private async void App_DMUpdatePosHandler(object sender, App.DMUpdatePosArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    foreach (SimpleChannel chn in channelCollection)
                    {
                        if (chn.Id == e.Id)
                        {
                            channelCollection.Remove(chn);
                            channelCollection.Insert(0, chn);
                            break;
                        }
                    }
                });
        }

        #endregion

        #region Channel

        private async void App_GuildChannelCreatedHandler(object sender, App.GuildChannelCreatedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (channelCollection.Count > 0)
                    {
                        SimpleChannel chn =
                            ChannelManager.MakeChannel(LocalState.Guilds[App.CurrentGuildId].channels[e.Channel.Id]);
                        if (chn != null)
                            channelCollection.Insert(0, chn);
                    }
                });
        }

        private async void App_GuildChannelDeletedHandler(object sender, App.GuildChannelDeletedArgs e)
        {
            await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (channelCollection.Count > 0)
                        for (int i = 0; i < channelCollection.Count; i++)
                            if (channelCollection[i].Id == e.ChannelId)
                            {
                                channelCollection.RemoveAt(i);
                                break;
                            }
                });
        }

        private void App_GuildChannelUpdatedHandler(object sender, App.GuildChannelUpdatedArgs e)
        {
            //App_GuildChannelDeletedHandler(sender, new App.GuildChannelDeletedArgs() { ChannelId = e.Channel.Id, GuildId = e.Channel.GuildId});
            //App_GuildChannelCreatedHandler(sender, new App.GuildChannelCreatedArgs() { Channel = e.Channel});
            foreach (SimpleChannel chn in channelCollection)
                if (chn.Id == e.Channel.Id)
                {
                    if (chn.Position == e.Channel.Position)
                    {
                        chn.Update(e.Channel);
                    }
                    else
                    {
                        channelCollection.Remove(chn);
                        App.GuildChannelCreated(e.Channel);
                    }

                    break;
                }
        }

        private int findLocation(SimpleChannel c)
        {
            if (c.Type == 0 || c.Type == 2 || c.Type == 4)
            {
                if (c.Type != 4)
                {
                    int pos = 0;
                    foreach (SimpleChannel chn in channelCollection)
                    {
                        if (chn.Id == c.ParentId)
                        {
                            if (c.Type == 0) return pos + c.Position + 1;

                            if (c.Type == 2)
                            {
                                //TODO: Handle Voice channels
                            }
                        }

                        pos++;
                    }
                }
            }
            else // type == 1 or 3
            {
                int pos = 0;
                foreach (SimpleChannel chn in channelCollection)
                {
                    if (Common.SnowflakeToTime(c.LastMessageId) > Common.SnowflakeToTime(chn.LastMessageId)) return pos;
                    pos++;
                }
            }

            return 0;
        }

        #endregion

        #region Guilds

        private async void App_GuildCreatedHandler(object sender, App.GuildCreatedArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { ServerList.Items.Insert(1 + TempGuildCount, GuildManager.CreateGuild(e.Guild)); });
        }

        private async void App_GuildSyncedHandler(object sender, GuildSync e)
        {
            MemberManager.ClearRoles();
            App.MemberListTrie = new PatriciaTrie<Common.AutoComplete>();
            if (!App.CurrentGuildIsDM && App.CurrentGuildId != null && App.CurrentGuildId == e.GuildId
            ) //Reduntant I know
            {
                //await GatewayManager.Gateway.RequestAllGuildMembers(App.CurrentGuildId);
                IEnumerable<GuildMember> members = e.Members;
                IEnumerable<Presence> presences = e.Presences;
                foreach (Presence presence in presences)
                    if (e.IsLarge && presence.Status == "offline")
                    {
                    }
                    else
                    {
                        if (LocalState.PresenceDict.ContainsKey(presence.User.Id))
                            LocalState.PresenceDict[presence.User.Id] = presence;
                        else
                            LocalState.PresenceDict.Add(presence.User.Id, presence);
                    }

                foreach (GuildMember member in members)
                {
                    member.setRoles(member.Roles
                        .TakeWhile(x =>
                            App.CurrentGuildId != null && LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(x))
                        .OrderByDescending(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Position));

                    bool added = LocalState.Guilds[App.CurrentGuildId].members.TryAdd(member.User.Id, member);
                    if (!added)
                        LocalState.Guilds[App.CurrentGuildId].members[member.User.Id] = member;

                    if (!string.IsNullOrEmpty(member.Nick))
                        App.MemberListTrie.Add(member.Nick.ToLower(),
                            new Common.AutoComplete(member.Nick, member.User.Username + "#" + member.User.Discriminator,
                                Common.AvatarString(member.User.Avatar, member.User.Id)));

                    App.MemberListTrie.Add(member.User.Username.ToLower(),
                        new Common.AutoComplete(member.User.Username + "#" + member.User.Discriminator, null,
                            Common.AvatarString(member.User.Avatar, member.User.Id)));
                }

                Debug.WriteLine("Trie traversal: " + App.MemberListTrie.Traversal() + "/end");
                if (LocalState.Guilds[App.CurrentGuildId].Raw.Roles != null)
                {
                    foreach (Role role in LocalState.Guilds[App.CurrentGuildId].Raw.Roles)
                    {
                        Role roleAlt = role;
                        //TODO Optimize the ConcurrentDictionary, because right now the .ContainsKey is running twice
                        if (LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(role.Id))
                            LocalState.Guilds[App.CurrentGuildId].roles[role.Id] = roleAlt;
                        else
                            LocalState.Guilds[App.CurrentGuildId].roles.TryAdd(role.Id, roleAlt);
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<Member> tempMembers = new List<Member>();
                    try
                    {
                        string[] keys = LocalState.Guilds[App.CurrentGuildId].members.Keys.ToArray();
                        for (int i = 0; keys.Count() > i; i++)
                        {
                            GuildMember member = LocalState.Guilds[App.CurrentGuildId].members[keys[i]];
                            //foreach(var member in LocalState.Guilds[App.CurrentGuildId].members)
                            // {
                            if (!e.IsLarge || LocalState.PresenceDict.ContainsKey(keys[i]))
                            {
                                Member m = new Member(member);
                                if (m.Raw.Roles != null)
                                    m.Raw.Roles = m.Raw.Roles
                                        .TakeWhile(x => LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(x))
                                        .OrderByDescending(x =>
                                            LocalState.Guilds[App.CurrentGuildId].roles[x].Position);

                                //Set it to first Hoist Role or everyone if null
                                //   await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                //  () =>
                                // {
                                m.MemberHoistRole = MemberManager.GetRole(
                                    m.Raw.Roles.FirstOrDefault(
                                        x => LocalState.Guilds[App.CurrentGuildId].roles[x].Hoist), App.CurrentGuildId);
                                
                                // });


                                if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                                    m.status = LocalState.PresenceDict[m.Raw.User.Id];
                                else
                                    m.status = new Presence {Status = "offline", Game = null};
                                if (member.Nick != null)
                                    m.DisplayName = member.Nick;
                                else
                                    m.DisplayName = member.User.Username;
                                // if (memberscvs(m.Raw.User.Id))
                                //{
                                //   memberscvs.Remove(m.Raw.User.Id);
                                //}
                                tempMembers.Add(m);
                            }
                        }

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            memberscvs =
                                new GroupedObservableCollection<HoistRole, Member>(c => c?.MemberHoistRole,
                                    tempMembers);
                            MembersCvs.Source = memberscvs;
                        });
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine(er.HResult + ": " + er.Message);
                    }

                    sw.Stop();
                    Debug.WriteLine("Itterating over all members took " + sw.ElapsedMilliseconds + "ms");
                    try
                    {
                        // var sortedMembers = memberscvs.I.OrderBy(m => (m)Raw.User.Username).GroupBy(m => ((Member)m.Value).MemberHoistRole).OrderByDescending(x => x.Key.Position);

                        //  foreach (var m in sortedMembers)
                        //  {
                        //      int count = m.Count();
                        //  }
                        //  await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        //      () =>
                        //      {
                        // MembersCVS = new CollectionViewSource();
                        //          MembersCvs.Source = sortedMembers;
                        //    });
                    }
                    catch
                    {
                    }

                    //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    //sw.Start();
                    //sw.Stop();
                    //else
                    //    MembersCVS.Source = memberscvs.SkipWhile(m => m.Value.status.Status == "offline").GroupBy(m => m.Value.MemberDisplayedRole).OrderBy(m => m.Key.Position).ToList();
                }
            }
        }

        #endregion

        #region Members

        private async void App_MembersUpdatedHandler(object sender, EventArgs e)
        {
            int totalrolecounter = 0;
            int everyonecounter = LocalState.Guilds[App.CurrentGuildId].members.Count() - totalrolecounter;

            if (LocalState.Guilds[App.CurrentGuildId].Raw.Roles != null)
                foreach (Role role in LocalState.Guilds[App.CurrentGuildId].Raw.Roles)
                {
                    Role roleAlt = role;
                    //TODO Optimize this, .ContainsKey is currently running twice
                    if (LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(role.Id))
                        LocalState.Guilds[App.CurrentGuildId].roles[role.Id] = roleAlt;
                    else
                        LocalState.Guilds[App.CurrentGuildId].roles.TryAdd(role.Id, roleAlt);
                }

            foreach (KeyValuePair<string, GuildMember> member in LocalState.Guilds[App.CurrentGuildId].members)
                if (LocalState.Guilds[App.CurrentGuildId].Raw.MemberCount < 1000 ||
                    (LocalState.PresenceDict.ContainsKey(member.Key) && LocalState.PresenceDict[member.Key].Status != "offline")) //Small guild
                {
                    Member m = new Member(member.Value);
                    m.Raw.Roles = m.Raw.Roles.TakeWhile(x => LocalState.Guilds[App.CurrentGuildId].roles.ContainsKey(x))
                        .OrderByDescending(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Position);

                    //Set it to first Hoist Role or everyone if null
                    m.MemberHoistRole =
                        MemberManager.GetRole(
                            m.Raw.Roles.FirstOrDefault(x => LocalState.Guilds[App.CurrentGuildId].roles[x].Hoist),
                            App.CurrentGuildId);

                    if (LocalState.PresenceDict.ContainsKey(m.Raw.User.Id))
                        m.status = LocalState.PresenceDict[m.Raw.User.Id];
                    else
                        m.status = new Presence {Status = "offline", Game = null};
                    // if (memberscvs.ContainsKey(m.Raw.User.Id))
                    // {
                    //     memberscvs.Remove(m.Raw.User.Id);
                    //  }
                    memberscvs.Add(m);
                }

            try
            {
                // App.DisposeMemberList(); //Clear all existing MemberList Items (cleanly)
                //     var sortedMembers =
                //         memberscvs.GroupBy(m => ((Member)m.Value).MemberHoistRole).OrderByDescending(x => x.Key.Position);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        // MembersCVS = new CollectionViewSource();
                        MembersCvs.Source = memberscvs;
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
            if ((UISize.CurrentState == Large) | (UISize.CurrentState == ExtraLarge))
                extrawidth = 240;
            ChannelHeader.MaxWidth = e.NewSize.Width - 72 * 3 + 1 + extrawidth;
        }

        private void UserStatus_Checked(object sender, RoutedEventArgs e)
        {
            if (UserStatusOnline.IsChecked == true)
                App.UpdatePresence("online");
            else if (UserStatusIdle.IsChecked == true)
                App.UpdatePresence("idle");
            else if (UserStatusDND.IsChecked == true)
                App.UpdatePresence("dnd");
            else
                App.UpdatePresence("invisible");
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            App.NavigateToSettings();
        }

        private void OpenAccountSettings(object sender, RoutedEventArgs e)
        {
            userFlyout.Hide();
            App.NavigateToAccountSettings();
        }

        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServerList.SelectedItem != null && ServerSelectionWasClicked)
            {
                ServerSelectionWasClicked = false;

                string guildid = (ServerList.SelectedItem as SimpleGuild).Id;
                App.NavigateToGuild(guildid);

                sideDrawer.OpenLeft();
                Task.Run(() => { UserActivityManager.SwitchSession(guildid); });
            }
        }

        private bool IgnoreChange;
        private bool lastChangeProgrammatic;
        private bool ChannelSelectionWasClicked = true;
        private bool ServerSelectionWasClicked = true;
        private object previousSelection;

        private void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelList.SelectedIndex != -1) Ad.Visibility = Visibility.Collapsed;

            //Verify if the last selection was navigated to (with a keyboard/controller) or actually clicked on

            //When selecting a category, we want to simulate ListView's Mode = Click, 
            //so we use IgnoreChange to immediately re-select the unselected item 
            //after having clicked on a category (without reloading anything)
            if (ChannelList.SelectedItem != null && ((ChannelList.SelectedItem as SimpleChannel).HavePermissions || (ChannelList.SelectedItem as SimpleChannel).Type == 4))
            {
                if (!lastChangeProgrammatic)
                {
                    if (!IgnoreChange) //True if the last selection was a category, Voice channel
                    {
                        if (ChannelSelectionWasClicked)
                        {
                            ChannelSelectionWasClicked =
                                false; //clearly it was, but the next one will not necessarily be clicked. So set to false.

                            if (App.ShowAds && !App.CinematicMode)
                            {
                                if (UISize.CurrentState == Large || UISize.CurrentState == ExtraLarge)
                                {
                                    PCAd.Visibility = Visibility.Visible;
                                    MobileAd.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    PCAd.Visibility = Visibility.Collapsed;
                                    MobileAd.Visibility = Visibility.Visible;
                                }
                            }

                            if (ChannelList.SelectedItem != null) //Called on clear
                            {
                                SimpleChannel channel = ChannelList.SelectedItem as SimpleChannel;
                                if (channel.Type == 4) //CATEGORY
                                {
                                    foreach (SimpleChannel item in channelCollection.Where(x =>
                                        (x as SimpleChannel).ParentId == channel.Id))
                                        if (item.Hidden)
                                            item.Hidden = false;
                                        else
                                            item.Hidden = true;
                                    channel.Hidden = !channel.Hidden;
                                    if (previousSelection == null)
                                        ChannelList.SelectedIndex = -1;
                                    else
                                        ChannelList.SelectedItem = previousSelection;
                                }
                                else if (channel.Type == 2) //VOICE
                                {
                                    IgnoreChange = true;
                                    App.ConnectToVoice(channel.Id, App.CurrentGuildId, channel.Name,
                                        LocalState.Guilds[App.CurrentGuildId].Raw.Name);
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
                                        string cid = (ChannelList.SelectedItem as SimpleChannel).Id;
                                        if (!string.IsNullOrEmpty(_autoselectchannelcontent))
                                            App.NavigateToDMChannel(cid, _autoselectchannelcontent,
                                                _autoselectchannelcontentsend);
                                        else
                                            App.NavigateToDMChannel(cid);
                                        Task.Run(() => { UserActivityManager.SwitchSession(cid); });
                                    }
                                    else
                                    {
                                        App.NavigateToGuildChannel(App.CurrentGuildId,
                                            (ChannelList.SelectedItem as SimpleChannel).Id);
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
        }

        private void ChannelList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as SimpleChannel).HavePermissions || (e.ClickedItem as SimpleChannel).Type == 4)
            {
                _autoselectchannel = null;
                ChannelSelectionWasClicked = true;
                if (e.ClickedItem == ChannelList.SelectedItem)
                    //This is for xbox one, because when "clicking" on a channel, it is already selected
                    ChannelList_SelectionChanged(null, null);
            }
        }

        private void ServerList_ItemClick(object sender, ItemClickEventArgs e)
        {
            _autoselectchannel = null;
            ServerSelectionWasClicked = true;
            if (e.ClickedItem == ServerList.SelectedItem)
                //This if for xbox one, because when clicking on a channel it is already selected
                ServerList_SelectionChanged(null, null);
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
                App.NavigateToChannelTopic(LocalState.Guilds[App.CurrentGuildId].channels[App.CurrentChannelId].raw);
        }

        private void ScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }

        private async void DisconnectedMask_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GatewayManager.Gateway.ConnectedSocket == false)
                if (NetworkInformation.GetInternetConnectionProfile()?.GetNetworkConnectivityLevel() ==
                    NetworkConnectivityLevel.InternetAccess)
                {
                    try
                    {
                        await GatewayManager.Gateway.ResumeAsync();
                    }
                    catch
                    {
                        App.CheckOnline();
                    }

                    if (GatewayManager.Gateway.ConnectedSocket)
                        _networkCheckTimer.Stop();
                    else
                        App.CheckOnline();
                }
        }
        #endregion

    }
}
