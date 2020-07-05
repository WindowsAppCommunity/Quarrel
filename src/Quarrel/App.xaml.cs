// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Helpers;
using Quarrel.Services.Settings;
using Quarrel.SubPages;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AcrylicBrush = Microsoft.UI.Xaml.Media.AcrylicBrush;
using DispatcherHelper = GalaSoft.MvvmLight.Threading.DispatcherHelper;

namespace Quarrel
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private static bool _resourcesLoaded;

        private static bool _hasSetup;

        static App()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddTransient<IConfiguration>(sp =>
            {
                var input = new Dictionary<string, string>
                {
                    { "Logging:LogLevel:Default", "Trace" },
                    { "Logging:LogLevel:System", "Information" },
                    { "Logging:LogLevel:Microsoft", "Information" },
                };

                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(input);
                return configurationBuilder.Build();
            });

            ServiceProvider = services.BuildServiceProvider();

            Configuration = ServiceProvider.GetService<IConfiguration>();

            var folder = ApplicationData.Current.LocalFolder;
            string fullPath = $"{folder.Path}\\Logs\\App.log";

            ServiceProvider.GetService<ILoggerFactory>().AddDebug((s, l) => l >= LogLevel.Information);
            ViewModelLocator = new ViewModelLocator();
        }

        private async void RegisterTask(string taskName)
        {
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder
            {
                Name = taskName,
            };

            var trigger = new ApplicationTrigger();
            taskBuilder.SetTrigger(trigger);

            var registration = taskBuilder.Register();
            await trigger.RequestAsync();
        }


        private async void RegisterBackgroundTask()
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "SuspendBackgroundTask")
                {
                    task.Value.Unregister(true);
                }
            }

            if (BackgroundTaskRegistration.AllTasks.All(cur => cur.Value.Name != "SuspendBackgroundTask") && backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                RegisterTask("SuspendBackgroundTask");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// This is the first line of authored code executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            SetupRequestedTheme();
            Suspending += OnSuspending;
            UnhandledException += App_UnhandledException;

            RegisterBackgroundTask();
        }

        /// <summary>
        /// Gets the Services for the app.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the Configurations for the app.
        /// </summary>
        public static IConfiguration Configuration { get; }

        /// <summary>
        /// Gets public access to the MainViewModel.
        /// </summary>
        public static ViewModelLocator ViewModelLocator { get; }

        /// <summary>
        /// Gets a value indicating whether or not the build is insider or release in runtime.
        /// </summary>

#if INSIDER
        public static bool IsInsiderBuild => true;
#else
        public static bool IsInsiderBuild => false;
#endif

        private ILogger Logger { get; } = App.ServiceProvider.GetService<ILogger<App>>();

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (_hasSetup)
            {
                return;
            }

            Frame rootFrame = Window.Current.Content as Frame;

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(400, 500));
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                (Current.Resources["AcrylicMessageBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicChannelPaneBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicGuildPaneBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicCommandBarBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicSubFrameBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
                (Current.Resources["AcrylicUserBackground"] as AcrylicBrush).TintLuminosityOpacity = 0.95;
            }

            SetupResources();
            RegisterMessages();
            SetupTitleBar();

            if (SystemInformation.DeviceFamily == "Windows.Xbox")
            {
                SetupCinematic();
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
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

            if (SystemInformation.DeviceFamily == "Windows.Xbox")
            {
                rootFrame.SizeChanged += ScaleDown;
            }
            _hasSetup = true;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = App.ServiceProvider.GetService<ILogger<App>>();

            logger?.LogCritical(default(EventId), e.Exception, "Unhandled exception crashed the app.");
            // Todo: Detect crash on next launch and handle appropriately
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();


            var status = await new ApplicationTrigger().RequestAsync();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Sets Resource settings according to settings.
        /// </summary>
        private void SetupResources()
        {
            // Stores the original SystemAccentColor
            Application.Current.Resources["OGSystemAccentColor"] = Application.Current.Resources["SystemAccentColor"];
            Application.Current.Resources["OGSystemAccentColorBrush"] = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);

            var settings = new SettingsService();

            // Set Accent Color brushes to blurple
            if (settings.Roaming.GetValue<bool>(SettingKeys.Blurple))
            {
                Application.Current.Resources["SystemAccentColor"] = Application.Current.Resources["BlurpleColor"];
                Application.Current.Resources["SystemControlBackgroundAccentBrush"] = Application.Current.Resources["Blurple"];
                Application.Current.Resources["SystemControlForegroundAccentBrush"] = Application.Current.Resources["Blurple"];
            }

            Color accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
            accentColor.A = 0;
            Application.Current.Resources["SystemAccentColorTransparent"] = accentColor;

            // Set Acrylic Fallback settings
            var acrylicSettings = settings.Roaming.GetValue<AcrylicSettings>(SettingKeys.AcrylicSettings);
            (App.Current.Resources["AcrylicMessageBackground"] as AcrylicBrush).AlwaysUseFallback = (acrylicSettings & AcrylicSettings.MessageView) != AcrylicSettings.MessageView;
            (App.Current.Resources["AcrylicChannelPaneBackground"] as AcrylicBrush).AlwaysUseFallback = (acrylicSettings & AcrylicSettings.ChannelView) != AcrylicSettings.ChannelView;
            (App.Current.Resources["AcrylicGuildPaneBackground"] as AcrylicBrush).AlwaysUseFallback = (acrylicSettings & AcrylicSettings.GuildView) != AcrylicSettings.GuildView;
            (App.Current.Resources["AcrylicCommandBarBackground"] as AcrylicBrush).AlwaysUseFallback = (acrylicSettings & AcrylicSettings.CommandBar) != AcrylicSettings.CommandBar;

            if (!_resourcesLoaded)
            {
                _resourcesLoaded = true;

                string style = "Default";
                if (settings.Roaming.GetValue<bool>(SettingKeys.FluentTheme))
                {
                    style = "Fluent";
                }

                var resourceDictionary = new ResourceDictionary { Source = new Uri($"ms-appx:///CustomStyles/{style}.xaml") };
                App.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        /// <summary>
        /// Registers message that change App Resources.
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<SettingChangedMessage<AcrylicSettings>>(this, m =>
            {
                if (m.Key == SettingKeys.AcrylicSettings)
                {
                    (App.Current.Resources["AcrylicMessageBackground"] as AcrylicBrush).AlwaysUseFallback = (m.Value & AcrylicSettings.MessageView) != AcrylicSettings.MessageView;
                    (App.Current.Resources["AcrylicChannelPaneBackground"] as AcrylicBrush).AlwaysUseFallback = (m.Value & AcrylicSettings.ChannelView) != AcrylicSettings.ChannelView;
                    (App.Current.Resources["AcrylicGuildPaneBackground"] as AcrylicBrush).AlwaysUseFallback = (m.Value & AcrylicSettings.GuildView) != AcrylicSettings.GuildView;
                    (App.Current.Resources["AcrylicCommandBarBackground"] as AcrylicBrush).AlwaysUseFallback = (m.Value & AcrylicSettings.CommandBar) != AcrylicSettings.CommandBar;
                }
            });

            // TODO: Full accent switch without restart or bust
        }

        private void SetupTitleBar()
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
                        statusBar.BackgroundColor = ((AcrylicBrush)Current.Resources["AcrylicCommandBarBackground"]).TintColor;
                        statusBar.ForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(default(EventId), ex, "Error caught accessing resources. (Group 1)");
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
                Logger.LogError(default(EventId), ex, "Error caught accessing resources (Group 2).");
            }
        }

        private void SetupRequestedTheme()
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

        private void SetupCinematic()
        {
            ApplicationViewScaling.TrySetDisableLayoutScaling(false);
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        private void ScaleDown(object sender, SizeChangedEventArgs e)
        {
            // Adjust DPI
            var rootFrame = Window.Current.Content as Frame;

            const double scale = 1.5;

            double scaledHeight = e.NewSize.Height * scale;
            double scaledWidth = e.NewSize.Width * scale;

            double bottomMargin = e.NewSize.Height - scaledHeight;
            double rightMargin = e.NewSize.Width - scaledWidth;
            rootFrame.Margin = new Thickness(0, 0, rightMargin / scale, bottomMargin / scale);

            double scaleXY = e.NewSize.Height / scaledHeight;
            ScaleTransform transform = rootFrame.GetTransform<ScaleTransform>();
            transform.ScaleX = transform.ScaleY = scaleXY;
        }
    }
}
