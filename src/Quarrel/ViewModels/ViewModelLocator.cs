using GalaSoft.MvvmLight.Ioc;
using Quarrel.Navigation;
using Quarrel.Services.Cache;
using Quarrel.Services.Clipboard;
using Quarrel.Services.DispatcherHelperEx;
using Quarrel.Services.Settings;
using Quarrel.Services.Users;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;
using Quarrel.SubPages;
using Quarrel.SubPages.UserSettings;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Guild;
using Quarrel.ViewModels.Services.Rest;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Users;
using Quarrel.ViewModels.Services.Voice;
using Quarrel.ViewModels.Services.Voice.Audio.In;
using Quarrel.ViewModels.Services.Voice.Audio.Out;
using System;
using Windows.ApplicationModel.Store;

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
            navigationService.Configure("AboutPage", typeof(AboutPage));
            navigationService.Configure("AddChannelPage", typeof(AddChannelPage));
            navigationService.Configure("AttachmentPage", typeof(AttachmentPage));
            navigationService.Configure("DiscordStatusPage", typeof(DiscordStatusPage));
            navigationService.Configure("LicensesPage", typeof(LicensesPage));
            navigationService.Configure("LoginPage", typeof(LoginPage));
            navigationService.Configure("SettingsPage", typeof(UserSettingsPage));
            navigationService.Configure("TopicPage", typeof(TopicPage));
            navigationService.Configure("UserProfilePage", typeof(UserProfilePage));
            navigationService.Configure("WhatsNewPage", typeof(WhatsNewPage));

            SimpleIoc.Default.Register<IDispatcherHelper, DispatcherHelperEx>();
            SimpleIoc.Default.Register<ISubFrameNavigationService>(() => navigationService);

            SimpleIoc.Default.Register<ICacheService, CacheService>();
            SimpleIoc.Default.Register<IClipboardService, ClipboardService>();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            SimpleIoc.Default.Register<IServiceProvider>(() => App.ServiceProvider);
            SimpleIoc.Default.Register<IGatewayService, GatewayService>();
            SimpleIoc.Default.Register<IDiscordService, DiscordService>();
            SimpleIoc.Default.Register<IGuildsService, GuildsService>();
            SimpleIoc.Default.Register<IAudioInService, AudioInService>();
            SimpleIoc.Default.Register<IAudioOutService, AudioOutService>();
            SimpleIoc.Default.Register<ICurrentUsersService, CurrentUsersService>();
            SimpleIoc.Default.Register<IVoiceService, VoiceService>();

            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive ||
                licenseInformation.ProductLicenses["Remove Ads"].IsActive ||
                licenseInformation.ProductLicenses["Polite Dontation"].IsActive ||
                licenseInformation.ProductLicenses["SignificantDontation"].IsActive ||
                licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive ||
                licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
            {
                SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.SetValue(SettingKeys.AdsRemoved, true);
            }
            else
            {
                // If none are active, set to false if not already set
                SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.SetValue(SettingKeys.AdsRemoved, false, false);
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }
        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();
    }
}
