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
using GalaSoft.MvvmLight.Threading;
using Quarrel.Services.Cache;
using Quarrel.Services.Gateway;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;

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
            this.InitializeComponent();
            this.Suspending += OnSuspending;
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

            SetupTitleBar();
            SimpleIoc.Default.Register<ICacheService, CacheService>();
            SimpleIoc.Default.Register<IGatewayService, GatewayService>();
            SimpleIoc.Default.Register<IDiscordService, DiscordService>();

            SimpleIoc.Default.Register<ICurrentUsersService, CurrentUsersService>();
            SimpleIoc.Default.Register<IVoiceService, VoiceService>();
            SimpleIoc.Default.Register<IAudioInService, AudioInService>();
            SimpleIoc.Default.Register<IAudioOutService, AudioOutService>();
            //Todo: viewmodel locator
            SimpleIoc.Default.GetInstance<ICurrentUsersService>();
            SimpleIoc.Default.GetInstance<IVoiceService>();

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
            DispatcherHelper.Initialize();
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
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = ((SolidColorBrush)Current.Resources["AcrylicCommandBarBackground"]).Color;
                    statusBar.ForegroundColor = ((SolidColorBrush)Current.Resources["MessageForeground"]).Color;
                }
            }

            view.TitleBar.ButtonForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
            view.TitleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)Current.Resources["MidBG"]).Color;
            view.TitleBar.ButtonHoverForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
            view.TitleBar.ButtonPressedBackgroundColor = ((SolidColorBrush)Current.Resources["LightBG"]).Color;
            view.TitleBar.ButtonPressedForegroundColor = ((SolidColorBrush)Current.Resources["Foreground"]).Color;
            view.TitleBar.ButtonInactiveForegroundColor = ((SolidColorBrush)Current.Resources["MidBG_hover"]).Color;
            view.TitleBar.InactiveForegroundColor = ((SolidColorBrush)Current.Resources["MidBG_hover"]).Color;
        }

        #endregion
    }
}
