using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.DispatcherHelperEx;
using Quarrel.Services.Gateway;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Settings;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;
using Quarrel.SubPages;
using Quarrel.SubPages.Settings;
using Quarrel.ViewModels.Services.DispatcherHelper;

namespace Quarrel.ViewModels
{

    /// <summary>
    /// Locates viewmodel
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var navigationService = new SubFrameNavigationService();
            navigationService.Configure("LoginPage", typeof(LoginPage));
            navigationService.Configure("SettingsPage", typeof(SettingsPage));
            navigationService.Configure("UserProfilePage", typeof(UserProfilePage));
            navigationService.Configure("AddChannelPage", typeof(AddChannelPage));

            SimpleIoc.Default.Register<IDispatcherHelper, DispatcherHelperEx>();
            SimpleIoc.Default.Register<ISubFrameNavigationService>(() => navigationService);

            SimpleIoc.Default.Register<ICacheService, CacheService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IGatewayService, GatewayService>();
            SimpleIoc.Default.Register<IDiscordService, DiscordService>();
            SimpleIoc.Default.Register<IGuildsService, GuildsService>();
            SimpleIoc.Default.Register<IAudioInService, AudioInService>();
            SimpleIoc.Default.Register<IAudioOutService, AudioOutService>();
            SimpleIoc.Default.Register<ICurrentUsersService, CurrentUsersService>();
            SimpleIoc.Default.Register<IVoiceService, VoiceService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
