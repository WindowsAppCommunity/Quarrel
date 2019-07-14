using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.Gateway;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;

namespace Quarrel.ViewModels
{

    /// <summary>
    /// Locates viewmodel
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ICacheService, CacheService>();
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
