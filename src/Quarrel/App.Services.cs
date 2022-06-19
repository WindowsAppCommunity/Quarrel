// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Services.Analytics;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.AppConnections;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Accounts;
using Quarrel.Services.Storage.Vault;
using Quarrel.Services.Versioning;
using Quarrel.Services.Windows;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Panels;
using Quarrel.ViewModels.SubPages;
using Quarrel.ViewModels.SubPages.DiscordStatus;
using Quarrel.ViewModels.SubPages.Host;
using Quarrel.ViewModels.SubPages.Meta;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings;
using Quarrel.ViewModels.SubPages.Settings.UserSettings;
using System;
using Windows.Storage;

namespace Quarrel
{
    partial class App
    {
        private IServiceProvider ConfigureServices()
        {
            // Register Services
            return new ServiceCollection()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .AddSingleton<IVersioningService, VersioningService>()
            .AddSingleton<IDiscordService, DiscordService>()
            .AddSingleton<IDispatcherService, DispatcherService>()
            .AddSingleton<IVaultService, VaultService>()
            .AddSingleton<IAccountInfoStorage, AccountInfoStorage>()
            .AddSingleton<IFileStorageService>(new FileStorageService(ApplicationData.Current.LocalFolder))
            .AddSingleton<IStorageService, StorageService>()
            .AddSingleton<IClipboardService, ClipboardService>()
            .AddSingleton<IWindowService, WindowService>()
            .AddSingleton<AppConnectionService>()

            // Other APIs
            .AddTransient<IGitHubService, GitHubService>()

            #if DEV
            .AddSingleton<ILoggingService, LoggingService>()
            #else
            .AddSingleton<ILoggingService, AppCenterService>()
            #endif

            // ViewModels
            .AddSingleton<WindowViewModel>()
            .AddSingleton<SubPageHostViewModel>()
            .AddTransient<LoginPageViewModel>()
            .AddSingleton<GuildsViewModel>()
            .AddSingleton<ChannelsViewModel>()
            .AddSingleton<CommandBarViewModel>()
            .AddSingleton<MessagesViewModel>()
            .AddSingleton<MessageBoxViewModel>()
            .AddSingleton<CurrentUserViewModel>()
            .AddSingleton<VoiceViewModel>()

            // SubPages
            .AddTransient<AboutPageViewModel>()
            .AddTransient<CreditPageViewModel>()
            .AddTransient<DiscordStatusViewModel>()
            .AddTransient<GuildSettingsPageViewModel>()
            .AddTransient<UserSettingsPageViewModel>()
            .BuildServiceProvider();
        }
    }
}
