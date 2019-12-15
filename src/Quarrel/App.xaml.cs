using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Services.Cache;
using Quarrel.Services.Gateway;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;
using Quarrel.ViewModels;
using Quarrel.Services.Settings;
using Quarrel.Services.Settings.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Windows.Storage;

namespace Quarrel
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; }
        public static IConfiguration Configuration { get; }

        public static ViewModelLocator ViewModelLocator => ServiceProvider.GetService<ViewModelLocator>();

        static App()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ViewModelLocator, ViewModelLocator>();
            services.AddLogging();

            services.AddTransient<IConfiguration>(sp =>
            {
                var input = new Dictionary<string, string>
                {
                    {"Logging:LogLevel:Default", "Trace"},
                    {"Logging:LogLevel:System", "Information"},
                    {"Logging:LogLevel:Microsoft", "Information"},
                };

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                //configurationBuilder.AddJsonFile("appsettings.json");
                configurationBuilder.AddInMemoryCollection(input);
                return configurationBuilder.Build();
            });

            ServiceProvider = services.BuildServiceProvider();

            Configuration = ServiceProvider.GetService<IConfiguration>();

            var folder = ApplicationData.Current.LocalFolder;
            string fullPath = $"{folder.Path}\\Logs\\App.log";

            //ServiceProvider.GetService<ILoggerFactory>().AddFile(fullPath, LogLevel.Debug);
           // ServiceProvider.GetService<ILoggerFactory>().AddDebug((s, l) => true);

        }

        private ILogger Logger { get; } = App.ServiceProvider.GetService<ILogger<App>>();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {            
            this.InitializeComponent();
            SetupRequestedTheme();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = App.ServiceProvider.GetService<ILogger<App>>();

            logger?.LogCritical(new EventId(), e.Exception, "Unhandled exception crashed the app.");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            Frame rootFrame = Window.Current.Content as Frame;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 500));
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                (Current.Resources["AcrylicMessageBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicChannelPaneBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicGuildPaneBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicCommandBarBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicSubFrameBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicUserBackground"] as Microsoft.UI.Xaml.Media.AcrylicBrush).TintLuminosityOpacity = 0.95;
            }
            SetupTitleBar();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            DispatcherHelper.Initialize();

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Content = new MainView(e.SplashScreen);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        #region Window Setup

        public void SetupTitleBar()
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = view.TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    try
                    {
                        statusBar.BackgroundOpacity = 1;
                        statusBar.BackgroundColor = ((Microsoft.UI.Xaml.Media.AcrylicBrush)Current.Resources["AcrylicCommandBarBackground"]).TintColor;
                        statusBar.ForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
                    }
                    catch (Exception ex) {
                        Logger.LogError(new EventId(), ex, "Error caught accessing resources. (Group 1)");
                    }
                }
            }

            try
            {
                view.TitleBar.ButtonForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
                view.TitleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)Current.Resources["MidBG"]).Color;
                view.TitleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
                view.TitleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Current.Resources["LightBG"]).Color;
                view.TitleBar.ButtonPressedForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
                view.TitleBar.ButtonInactiveForegroundColor = ((SolidColorBrush)Current.Resources["MidBG_hover"]).Color;
                view.TitleBar.InactiveForegroundColor = ((SolidColorBrush)Current.Resources["MidBG_hover"]).Color;
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId(), ex, "Error caught accessing resources (Group 2).");
            }
        }

        public void SetupRequestedTheme()
        {

            switch (new SettingsService().Roaming.GetValue<Theme>(SettingKeys.Theme))
            {
                case Theme.Dark:
                    Application.Current.RequestedTheme = ApplicationTheme.Dark;
                    break;
                case Theme.Light:
                    Application.Current.RequestedTheme = ApplicationTheme.Light;
                    break;

                default:
                    Application.Current.RequestedTheme = ApplicationTheme.Dark;
                    break;
            }

            Logger.LogDebug($"Theme is: {Application.Current.RequestedTheme}");
        }

        #endregion
    }
}
