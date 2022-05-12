// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using OwlCore.AbstractStorage;
using Quarrel.Services.Analytics;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.AppConnections;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Models;
using Quarrel.Services.Versioning;
using Quarrel.Services.Windows;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Panels;
using Quarrel.ViewModels.SubPages;
using Quarrel.ViewModels.SubPages.DiscordStatus;
using Quarrel.ViewModels.SubPages.Host;
using Quarrel.ViewModels.SubPages.Meta;
using Quarrel.ViewModels.SubPages.Settings;
using System;
using Windows.Storage;

namespace Quarrel
{
    partial class App
    {
        private IServiceProvider ConfigureServices()
        {
            IFolderData appDataFolder = new FolderData(ApplicationData.Current.LocalFolder);

            // Register Services
            var services = new ServiceCollection();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IVersioningService, VersioningService>();
            services.AddSingleton<IDiscordService, DiscordService>();
            services.AddSingleton<IDispatcherService, DispatcherService>();
            services.AddSingleton<IStorageService>(new StorageService(appDataFolder, JsonAsyncSerializer.Singleton));
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<AppConnectionService>();

            // Other APIs
            services.AddTransient<IGitHubService, GitHubService>();

            #if DEV
            services.AddSingleton<IAnalyticsService, LoggingAnalyticsService>();
            #else
            services.AddSingleton<IAnalyticsService, AppCenterService>();
            #endif

            // ViewModels
            services.AddSingleton<WindowViewModel>();
            services.AddSingleton<SubPageHostViewModel>();
            services.AddTransient<LoginPageViewModel>();
            services.AddSingleton<GuildsViewModel>();
            services.AddSingleton<ChannelsViewModel>();
            services.AddSingleton<CommandBarViewModel>();
            services.AddSingleton<MessagesViewModel>();
            services.AddSingleton<MessageBoxViewModel>();
            services.AddSingleton<CurrentUserViewModel>();

            // SubPages
            services.AddTransient<AboutPageViewModel>();
            services.AddTransient<CreditPageViewModel>();
            services.AddTransient<DiscordStatusViewModel>();
            services.AddTransient<UserSettingsPageViewModel>();

            #if DEV
            ApplyDitryOverrides(services);
            #endif

            return services.BuildServiceProvider();
        }

        #if DEV
        private void ApplyDitryOverrides(ServiceCollection services)
        {
            // Fill with dirty service overrides for stress testing.
        }
        #endif
    }
}
