// Quarrel © 2022

using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Services.Windows
{
    public class WindowService : IWindowService
    {
        public async void OpenSecondaryWindow()
        {
            var currentAppView = ApplicationView.GetForCurrentView();
            var newCoreAppView = CoreApplication.CreateNewView();
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var newWindow = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = "Secondary Window";

                newWindow.Content = new TextBlock()
                {
                    Text = "Secondary Window Content",
                };

                newWindow.Activate();

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppView.Id);
            });
        }
    }
}
