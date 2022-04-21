// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Controls.Host;
using Quarrel.Helpers.LaunchArgs.Models;
using Quarrel.Messages;
using Quarrel.Services.Localization;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
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
            Launch(e.PrelaunchActivated, null);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            if (args.Kind == ActivationKind.Protocol && args is ProtocolActivatedEventArgs protocolArgs)
            {
                LaunchArgsBase? launchArgs = LaunchArgsBase.Parse(protocolArgs.Uri.ToString());
                Launch(false, launchArgs);
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);

            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails appServiceTriggerDetails)
            {
                SetupAppServiceConnection(appServiceTriggerDetails);
            }
        }

        private void Launch(bool isPrelaunchActivated, LaunchArgsBase? args = null)
        {
            if (Window.Current.Content is not WindowHost)
            {
                InitializeUI();
            }

            if (!isPrelaunchActivated)
            {
                Window.Current.Activate();
            }

            if (args is not null)
            {
                IMessenger messenger = Services.GetRequiredService<IMessenger>();
                messenger.Register<GuildsLoadedMessage>(this, (_,_) =>
                {
                    messenger.Unregister<GuildsLoadedMessage>(this);
                    args.RunPostLoad(Services);
                });
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
    }
}
