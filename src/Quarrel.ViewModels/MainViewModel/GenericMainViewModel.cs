// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Relationships;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Clipboard;
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
using Quarrel.ViewModels.Services.Voice;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The ViewModel for all data throughout the app.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ICacheService _cacheService;
        private readonly IChannelsService _channelsService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherHelper _dispatcherHelper;
        private readonly IClipboardService _clipboardService;
        private readonly IGatewayService _gatewayService;
        private readonly IGuildsService _guildsService;
        private readonly IFriendsService _friendsService;
        private readonly IPresenceService _presenceService;
        private readonly ISettingsService _settingsService;
        private readonly IVoiceService _voiceService;
        private readonly ISubFrameNavigationService _subFrameNavigationService;
        private RelayCommand openAbout;
        private RelayCommand openCredit;
        private RelayCommand openSettings;
        private RelayCommand openWhatsNew;
        private RelayCommand disconnectVoiceCommand;
        private VoiceState voiceState = new VoiceState();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="analyticsService">The app's analytics service.</param>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="settingsService">The app's settings service.</param>
        /// <param name="channelsService">The app's channel service.</param>
        /// <param name="discordService">The app's discord service.</param>
        /// <param name="currentUserService">The app's current user service.</param>
        /// <param name="gatewayService">The app's gateway service.</param>
        /// <param name="presenceService">The app's presence service.</param>
        /// <param name="guildsService">The app's guilds service.</param>
        /// <param name="subFrameNavigationService">The app's subframe navigation service.</param>
        /// <param name="friendsService">The app's friends service.</param>
        /// <param name="dispatcherHelper">The app's dispatcher helper.</param>
        /// <param name="voiceService">The app's voice service.</param>
        /// <param name="clipboardService">The app's clipboard service.</param>
        /// <remarks>Takes all service parameters from ViewModel Locator.</remarks>
        public MainViewModel(
            IAnalyticsService analyticsService,
            ICacheService cacheService,
            ISettingsService settingsService,
            IChannelsService channelsService,
            IDiscordService discordService,
            ICurrentUserService currentUserService,
            IGatewayService gatewayService,
            IPresenceService presenceService,
            IGuildsService guildsService,
            ISubFrameNavigationService subFrameNavigationService,
            IFriendsService friendsService,
            IDispatcherHelper dispatcherHelper,
            IVoiceService voiceService,
            IClipboardService clipboardService)
        {
            _analyticsService = analyticsService;
            _cacheService = cacheService;
            _settingsService = settingsService;
            _discordService = discordService;
            _currentUserService = currentUserService;
            _channelsService = channelsService;
            _friendsService = friendsService;
            _presenceService = presenceService;

            _gatewayService = gatewayService;
            _guildsService = guildsService;
            _subFrameNavigationService = subFrameNavigationService;
            _dispatcherHelper = dispatcherHelper;
            _voiceService = voiceService;
            _clipboardService = clipboardService;

            RegisterGenericMessages();
            RegisterChannelsMessages();
            RegisterGuildsMessages();
            RegisterMembersMessages();
            RegisterMessagesMessages();
        }

        /// <summary>
        /// Gets a command that opens the About SubPage.
        /// </summary>
        public RelayCommand OpenAbout => openAbout = new RelayCommand(() =>
        {
            Task.Run(() => _subFrameNavigationService.NavigateTo("AboutPage"));
        });

        /// <summary>
        /// Gets a command that opens the Credit SubPage.
        /// </summary>
        public RelayCommand OpenCredit => openCredit = new RelayCommand(() =>
        {
            Task.Run(() => _subFrameNavigationService.NavigateTo("CreditPage"));
        });

        /// <summary>
        /// Gets a command that opens the Settings SubPage.
        /// </summary>
        public RelayCommand OpenSettings => openSettings = new RelayCommand(() =>
        {
            Task.Run(() =>
            {
                _subFrameNavigationService.NavigateTo("UserSettingsPage");
                _analyticsService.Log(Constants.Analytics.Events.OpenUserSettings);
            });
        });

        /// <summary>
        /// Gets a command that opens the What's New SubPage.
        /// </summary>
        public RelayCommand OpenWhatsNew => openWhatsNew = new RelayCommand(() =>
        {
            Task.Run(() => _subFrameNavigationService.NavigateTo("WhatsNewPage"));
        });

        /// <summary>
        /// Gets a command that sets VoiceStatus to null.
        /// </summary>
        public RelayCommand DisconnectVoiceCommand => disconnectVoiceCommand = disconnectVoiceCommand ?? new RelayCommand(() =>
        {
            Task.Run(() => _gatewayService.Gateway.VoiceStatusUpdate(null, null, false, false));
        });

        /// <summary>
        /// Register for MVVM Messenger messages in MainViewModel.
        /// </summary>
        private void RegisterGenericMessages()
        {
            // Failed to login in due to invalid token
            // Deletes token and asks user to sign in again
            MessengerInstance.Register<GatewayInvalidSessionMessage>(this, async _ =>
            {
                await _cacheService.Persistent.Roaming.DeleteValueAsync(Constants.Cache.Keys.AccessToken);
                Login();
            });

            // Ready Message recieved. Setsup Friend Collections and navigates to DM guild
            MessengerInstance.Register<GatewayReadyMessage>(this, _ =>
            {
                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    BindableCurrentFriends.AddRange(_friendsService.Friends.Values.Where(x => x.IsFriend));
                    BindablePendingFriends.AddRange(
                        _friendsService.Friends.Values.Where(x => x.IsIncoming || x.IsOutgoing));
                    BindableBlockedUsers.AddRange(_friendsService.Friends.Values.Where(x => x.IsBlocked));
                });
            });

            MessengerInstance.Register<GatewayRelationshipAddedMessage>(this, x =>
            {
                var friend = new BindableFriend(x.Friend);
                _friendsService.Friends.TryAdd(friend.RawModel.Id, friend);

                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    switch (x.Friend.Type)
                    {
                        case 1:
                            BindableCurrentFriends.Add(friend);
                            break;
                        case 2:
                            BindableBlockedUsers.Add(friend);
                            break;
                        case 3:
                        case 4:
                            BindablePendingFriends.Add(friend);
                            break;
                    }
                });
            });

            MessengerInstance.Register<GatewayRelationshipRemovedMessage>(this, x =>
            {
                BindableFriend friend;
                _friendsService.Friends.TryRemove(x.Friend.Id, out friend);

                _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (friend != null)
                    {
                        switch (friend.Model.Type)
                        {
                            case 1:
                                BindableCurrentFriends.Remove(friend);
                                break;
                            case 2:
                                BindableBlockedUsers.Remove(friend);
                                break;
                            case 3:
                            case 4:
                                BindablePendingFriends.Remove(friend);
                                break;
                        }
                    }
                });
            });

            MessengerInstance.Register<GatewayRelationshipUpdatedMessage>(this, x =>
            {
                BindableFriend friend;
                _friendsService.Friends.TryGetValue(x.Friend.Id, out friend);

                if (friend != null)
                {
                    _dispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        switch (friend.Model.Type)
                        {
                            case 1:
                                BindableCurrentFriends.Remove(friend);
                                break;
                            case 2:
                                BindableBlockedUsers.Remove(friend);
                                break;
                            case 3:
                            case 4:
                                BindablePendingFriends.Remove(friend);
                                break;
                        }

                        friend.Model = x.Friend;
                        switch (x.Friend.Type)
                        {
                            case 1:
                                BindableCurrentFriends.Add(friend);
                                break;
                            case 2:
                                BindableBlockedUsers.Add(friend);
                                break;
                            case 3:
                            case 4:
                                BindablePendingFriends.Add(friend);
                                break;
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Logins in with cached token or opens login page.
        /// </summary>
        public async void Login()
        {
            string token =
                (string)await _cacheService.Persistent.Roaming.TryGetValueAsync<object>(
                    Constants.Cache.Keys.AccessToken);
            if (string.IsNullOrEmpty(token))
            {
                _subFrameNavigationService.NavigateTo("LoginPage");
            }
            else
            {
                await _discordService.Login(token);
            }
        }

        public VoiceState VoiceState
        {
            get => voiceState;
            set
            {
                Set(ref voiceState, value);
                RaisePropertyChanged(nameof(IsConnectedToVoiceChannel));
            }
        }

        /// <summary>
        /// Gets a value indicating whether a user is connected to a voice channel.
        /// </summary>
        public bool IsConnectedToVoiceChannel => VoiceState.ChannelId != null;

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindableCurrentFriends { get; set; } =
            new ObservableRangeCollection<BindableFriend>();

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindablePendingFriends { get; set; } =
            new ObservableRangeCollection<BindableFriend>();

        [NotNull]
        public ObservableRangeCollection<BindableFriend> BindableBlockedUsers { get; set; } =
            new ObservableRangeCollection<BindableFriend>();
    }
}