using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Quarrel.Controls.Shell;
using Quarrel.Services.Analytics;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Localization;
using Quarrel.ViewModels.SubPages;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Quarrel
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private ILocalizationService? _localizationService;
        private IAnalyticsService? _analyticsService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            InitailizeRequiredServices();

            if (!(Window.Current.Content is QuarrelHost))
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
            Guard.IsNotNull(_localizationService, nameof(_localizationService));

            FrameworkElement root = new QuarrelHost();
            Window.Current.Content = root;

            // Handle flow direction
            root.FlowDirection = _localizationService.IsRightToLeftLanguage ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void InitailizeRequiredServices()
        {
            // Initialize services
            _analyticsService = new LoggingAnalyticsService(); // TODO: AppCenter Analytics
            _localizationService = new LocalizationService();

            // Register Services
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(_analyticsService);
            services.AddSingleton(_localizationService);

            // Register ViewModel transients
            services.AddTransient<LoginPageViewModel>();

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());
        }
    }
}
