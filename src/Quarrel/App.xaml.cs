﻿// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using OwlCore.AbstractStorage;
using Quarrel.Controls.Shell;
using Quarrel.Services.Analytics;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.Services.Storage.Models;
using Quarrel.Services.Versioning;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Panels;
using Quarrel.ViewModels.SubPages;
using Quarrel.ViewModels.SubPages.DiscordStatus;
using Quarrel.ViewModels.SubPages.Host;
using Quarrel.ViewModels.SubPages.Meta;
using System;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Quarrel
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Services = ConfigureServices();

            // TODO: This should not be needed
            Ioc.Default.ConfigureServices(Services);

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (Window.Current.Content is not WindowHost)
            {
                InitializeUI();
            }

            if (!e.PrelaunchActivated)
            {
                Window.Current.Activate();
            }
        }

        private void InitializeUI()
        {
            FrameworkElement root = new WindowHost();
            Window.Current.Content = root;

            // Handle flow direction
            ILocalizationService localizationService = Services.GetRequiredService<ILocalizationService>();
            root.FlowDirection = localizationService.IsRightToLeftLanguage ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

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

            // Other APIs
            services.AddTransient<IGitHubService, GitHubService>();

            // TODO: Release analytics services
            services.AddSingleton<IAnalyticsService, LoggingAnalyticsService>();

            // ViewModels
            services.AddSingleton<WindowViewModel>();
            services.AddSingleton<SubPageHostViewModel>();
            services.AddTransient<LoginPageViewModel>();
            services.AddSingleton<GuildsViewModel>();
            services.AddSingleton<ChannelsViewModel>();
            services.AddSingleton<MessagesViewModel>();
            services.AddSingleton<CurrentUserViewModel>();

            // SubPages
            services.AddTransient<AboutPageViewModel>();
            services.AddTransient<CreditPageViewModel>();
            services.AddTransient<DiscordStatusViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
