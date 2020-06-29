// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Voice;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Helpers;
using Quarrel.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.Clipboard;
using Quarrel.Services.DispatcherHelperEx;
using Quarrel.Services.Resources;
using Quarrel.Services.Settings;
using Quarrel.SubPages;
using Quarrel.SubPages.AddServer;
using Quarrel.SubPages.GuildSettings;
using Quarrel.SubPages.UserSettings;
using Quarrel.ViewModels.Controls;
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
using Quarrel.ViewModels.Services.Resources;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Voice;
using System;
using System.Threading.Tasks;
using WebRTCBackgroundTask;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// Initializes the ViewModel and Services.
    /// </summary>
    public class ViewModelLocator
    {
        private static bool _hasRun = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class and the <see cref="MainViewModel"/>.
        /// Creates and registers all the services with <see cref="SimpleIoc.Default"/>.
        /// </summary>
        public ViewModelLocator()
        {
            if (_hasRun)
            {
                return;
            }

            _hasRun = true;

            SimpleIoc.Default.Register<IDispatcherHelper, DispatcherHelperEx>();

            var navigationService = new SubFrameNavigationService();
            navigationService.Configure("AboutPage", typeof(AboutPage));
            navigationService.Configure("AddChannelPage", typeof(AddChannelPage));
            navigationService.Configure("AddServerPage", typeof(AddServerPage));
            navigationService.Configure("AttachmentPage", typeof(AttachmentPage));
            navigationService.Configure("ChangeNicknamePage", typeof(ChangeNicknamePage));
            navigationService.Configure("CreateInvitePage", typeof(CreateInvitePage));
            navigationService.Configure("CreditPage", typeof(CreditPage));
            navigationService.Configure("DiscordStatusPage", typeof(DiscordStatusPage));
            navigationService.Configure("GuildSettingsPage", typeof(GuildSettingsPage));
            navigationService.Configure("LicensesPage", typeof(LicensesPage));
            navigationService.Configure("LoginPage", typeof(LoginPage));
            navigationService.Configure("TopicPage", typeof(TopicPage));
            navigationService.Configure("UserProfilePage", typeof(UserProfilePage));
            navigationService.Configure("UserSettingsPage", typeof(UserSettingsPage));
            navigationService.Configure("WhatsNewPage", typeof(WhatsNewPage));
            SimpleIoc.Default.Register<ISubFrameNavigationService>(() => navigationService);

            AppCenterService appCenterService = new AppCenterService();
#if !DEBUG
            appCenterService.Initialize();
#endif
            SimpleIoc.Default.Register<IAnalyticsService>(() => appCenterService);

            SimpleIoc.Default.Register<ICacheService, CacheService>();
            SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
            SimpleIoc.Default.Register<IResourceService, ResourceService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IServiceProvider>(() => App.ServiceProvider);
            SimpleIoc.Default.Register<IGatewayService, GatewayService>();
            SimpleIoc.Default.Register<IDiscordService, DiscordService>();
            SimpleIoc.Default.Register<IPresenceService, PresenceService>();
            SimpleIoc.Default.Register<IFriendsService, FriendsService>();
            SimpleIoc.Default.Register<IChannelsService, ChannelsService>();
            SimpleIoc.Default.Register<IGuildsService, GuildsService>();
            SimpleIoc.Default.Register<ICurrentUserService, CurrentUsersService>();
            SimpleIoc.Default.Register<IVoiceService, VoiceService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<EmojiPickerViewModel>(() =>
            {
                var tmp = Task.Run(async () => await Constants.FromFile.GetEmojiLists());
                return new EmojiPickerViewModel(tmp.GetAwaiter().GetResult());
            });
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        /// <summary>
        /// Gets the <see cref="EmojiPickerViewModel"/>.
        /// </summary>
        public EmojiPickerViewModel EmojiPicker => SimpleIoc.Default.GetInstance<EmojiPickerViewModel>();
    }
}
