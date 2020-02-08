using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Messages.Posts.Requests;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Friends;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Settings;
using System.Collections.ObjectModel;
using System.Linq;
using Quarrel.ViewModels.Services.Clipboard;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Creates default MainViewModel with all Messenger events registered
        /// </summary>
        /// <remarks>Takes all service parameters from ViewModel Locator</remarks>
        public MainViewModel(ICacheService cacheService, ISettingsService settingsService, IChannelsService channelsService,
            IDiscordService discordService, ICurrentUserService currentUserService, IGatewayService gatewayService, IPresenceService presenceService,
            IGuildsService guildsService, ISubFrameNavigationService subFrameNavigationService, IFriendsService friendsService,
            IDispatcherHelper dispatcherHelper, IClipboardService clipboardService)
        {
            CacheService = cacheService;
            SettingsService = settingsService;
            DiscordService = discordService;
            CurrentUserService = currentUserService;
            ChannelsService = channelsService;
            FriendsService = friendsService;
            PresenceService = presenceService;

            GatewayService = gatewayService;
            GuildsService = guildsService;
            SubFrameNavigationService = subFrameNavigationService;
            DispatcherHelper = dispatcherHelper;
            ClipboardService = clipboardService;

            RegisterGenericMessages();
            RegisterChannelsMessages();
            RegisterGuildsMessages();
            RegisterMembersMessages();
            RegisterMessagesMessages();
        }

        #endregion

        #region Commands

        #region Navigation

        /// <summary>
        /// Opens the About SubPage
        /// </summary>
        public RelayCommand OpenAbout => openAbout = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("AboutPage");
        });
        private RelayCommand openAbout;

        /// <summary>
        /// Opens the Credit SubPage
        /// </summary>
        public RelayCommand OpenCredit => openCredit = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("CreditPage");
        });
        private RelayCommand openCredit;

        /// <summary>
        /// Opens the Settings SubPage
        /// </summary>
        public RelayCommand OpenSettings => openSettings = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("UserSettingsPage");
        });
        private RelayCommand openSettings;

        /// <summary>
        /// Opens the What's New SubPage
        /// </summary>
        public RelayCommand OpenWhatsNew => openWhatsNew = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("WhatsNewPage");
        });
        private RelayCommand openWhatsNew;

        #endregion

        #region Voice

        /// <summary>
        /// Set VoiceStatus to null
        /// </summary>
        private RelayCommand disconnectVoiceCommand;
        public RelayCommand DisconnectVoiceCommand => disconnectVoiceCommand = disconnectVoiceCommand ?? new RelayCommand(async () =>
        {
            await GatewayService.Gateway.VoiceStatusUpdate(null, null, false, false);
        });

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Register for MVVM Messenger messages in MainViewModel
        /// </summary>
        private void RegisterGenericMessages()
        {
            #region Gateway 

            #region Initialize

            // Failed to login in due to invalid token
            // Deletes token and asks user to sign in again
            MessengerInstance.Register<GatewayInvalidSessionMessage>(this, async _ =>
            {
                await CacheService.Persistent.Roaming.DeleteValueAsync(Constants.Cache.Keys.AccessToken);
                Login();
            });

            // Ready Message recieved. Setsup Friend Collections and navigates to DM guild
            MessengerInstance.Register<GatewayReadyMessage>(this, _ =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    MessengerInstance.Send(new GuildNavigateMessage(GuildsService.AllGuilds["DM"]));

                    // Show guilds
                    BindableCurrentFriends.AddRange(FriendsService.Friends.Values.Where(x => x.IsFriend));
                    BindablePendingFriends.AddRange(
                        FriendsService.Friends.Values.Where(x => x.IsIncoming || x.IsOutgoing));
                    BindableBlockedUsers.AddRange(FriendsService.Friends.Values.Where(x => x.IsBlocked));
                });
            });

            #endregion

            #endregion

            // Allows request for result from VoiceState
            MessengerInstance.Register<CurrentUserVoiceStateRequestMessage>(this, m => { DispatcherHelper.CheckBeginInvokeOnUi(() => m.ReportResult(VoiceState)); });
        }

        /// <summary>
        /// Logins in with cached token or opens login page
        /// </summary>
        public async void Login()
        {
            string token =
                (string)await CacheService.Persistent.Roaming.TryGetValueAsync<object>(
                    Constants.Cache.Keys.AccessToken);
            if (string.IsNullOrEmpty(token))
                SubFrameNavigationService.NavigateTo("LoginPage");
            else
                await DiscordService.Login(token);
        }

        #endregion

        #region Properties

        #region Services

        private readonly ICacheService CacheService;
        private readonly IChannelsService ChannelsService;
        private readonly ICurrentUserService CurrentUserService;
        private readonly IDiscordService DiscordService;
        private readonly IDispatcherHelper DispatcherHelper;
        private readonly IClipboardService ClipboardService;
        private readonly IGatewayService GatewayService;
        private readonly IGuildsService GuildsService;
        private readonly IFriendsService FriendsService;
        private readonly IPresenceService PresenceService;
        private readonly ISettingsService SettingsService;
        private readonly ISubFrameNavigationService SubFrameNavigationService;

        #endregion

        public VoiceState VoiceState
        {
            get => voiceState;
            set => Set(ref voiceState, value);
        }
        private VoiceState voiceState = new VoiceState();

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindableCurrentFriends { get; set; } =
            new ObservableRangeCollection<BindableFriend>();

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindablePendingFriends { get; set; } =
            new ObservableRangeCollection<BindableFriend>();

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindableBlockedUsers { get; set; } =
            new ObservableRangeCollection<BindableFriend>();

        #endregion
    }
}