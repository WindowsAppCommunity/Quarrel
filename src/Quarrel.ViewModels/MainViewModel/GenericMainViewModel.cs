using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Messages.Posts.Requests;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Guild;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Rest;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Group = DiscordAPI.Models.Group;

namespace Quarrel.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Creates default MainViewModel with all Messenger events registered
        /// </summary>
        /// <remarks>Takes all service parameters from ViewModel Locator</remarks>
        public MainViewModel(ICacheService cacheService, ISettingsService settingsService,
            IDiscordService discordService, ICurrentUsersService currentUsersService, IGatewayService gatewayService,
            IGuildsService guildsService, ISubFrameNavigationService subFrameNavigationService,
            IDispatcherHelper dispatcherHelper)
        {
            CacheService = cacheService;
            SettingsService = settingsService;
            DiscordService = discordService;
            CurrentUsersService = currentUsersService;
            GatewayService = gatewayService;
            GuildsService = guildsService;
            SubFrameNavigationService = subFrameNavigationService;
            DispatcherHelper = dispatcherHelper;

            RegisterGenericMessages();
            RegisterChannelsMessages();
            RegisterGuildsMessages();
            RegisterMembersMessages();
            RegisterMessagesMessages();
        }

        #endregion

        #region Commands

        #region Navigation

        private RelayCommand openSettings;
        public RelayCommand OpenSettings => openSettings = new RelayCommand(() =>
        {
            SubFrameNavigationService.NavigateTo("SettingsPage");
        });

        #endregion

        #region Voice

        /// <summary>
        /// Set VoiceStatus to null
        /// </summary>
        private RelayCommand disconnectVoiceCommand;
        public RelayCommand DisconnectVoiceCommand => disconnectVoiceCommand ??= new RelayCommand(async () =>
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
                    MessengerInstance.Send(new GuildNavigateMessage(GuildsService.Guilds["DM"]));

                    // Show guilds
                    BindableCurrentFriends.AddRange(CurrentUsersService.Friends.Values.Where(x => x.IsFriend));
                    BindablePendingFriends.AddRange(
                        CurrentUsersService.Friends.Values.Where(x => x.IsIncoming || x.IsOutgoing));
                    BindableBlockedUsers.AddRange(CurrentUsersService.Friends.Values.Where(x => x.IsBlocked));
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
        private readonly ISettingsService SettingsService;
        private readonly IDiscordService DiscordService;
        public readonly ICurrentUsersService CurrentUsersService;
        private readonly IGatewayService GatewayService;
        private readonly IGuildsService GuildsService;
        private readonly ISubFrameNavigationService SubFrameNavigationService;
        private readonly IDispatcherHelper DispatcherHelper;

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