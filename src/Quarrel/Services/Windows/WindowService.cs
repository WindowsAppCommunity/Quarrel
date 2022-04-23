// Quarrel © 2022

using System;
using Quarrel.Controls.Host;
using Quarrel.Services.Localization;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Quarrel.Services.Windows
{
    public class WindowService : IWindowService
    {
        private readonly ILocalizationService _localizationService;

        public WindowService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <inheritdoc/>
        public async void OpenSecondaryWindow()
        {
            var currentAppView = ApplicationView.GetForCurrentView();
            var newCoreAppView = CoreApplication.CreateNewView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var newWindow = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = "Secondary Window";

                FrameworkElement root = new SecondaryWindowHost
                {
                    FlowDirection = _localizationService.IsRightToLeftLanguage ? FlowDirection.RightToLeft : FlowDirection.LeftToRight
                };

                newWindow.Content = root;
                newWindow.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id);
            });
        }
    }
}
