// Special thanks to Sergio Pedri for the basis of this design

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Messages.Navigation.SubFrame;
using Quarrel.ViewModels.Services.Navigation;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel.SubPages.Host
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubFrameHost : Page
    {
        public SubFrameHost()
        {
            this.InitializeComponent();
            this.RootGrid.Visibility = Visibility.Collapsed;
            this.SizeChanged += (s, e) => UpdateLayout(e.NewSize);

            // Navigation
            Messenger.Default.Register<SubFrameNavigationRequestMessage>(this, m => DisplaySubFramePage(m.SubPage as UserControl));
            Messenger.Default.Register<SubFrameCloseRequestMessage>(this, _ => CloseSubFramePage());
            SystemNavigationManager.GetForCurrentView().BackRequested += SubFrameControl_BackRequested;
        }

        #region Sub page navigation

        // Synchronization semaphore slim to avoid race conditions for user requests
        private readonly SemaphoreSlim SubFrameSemaphoreSlim = new SemaphoreSlim(1, 1);

        // Displays a page in the popup frame
        private async void DisplaySubFramePage([NotNull] UserControl subPage)
        {
            await SubFrameSemaphoreSlim.WaitAsync();
            try
            {
                // Fade out the current content, if present
                if (SubPage is UserControl page)
                {
                    page.IsHitTestVisible = false;
                    SubFrameContentHost.Visibility = Visibility.Collapsed;
                    await Task.Delay(400); // Time for the animations to complete
                    SubPage = subPage;
                    Messenger.Default.Send(new SubFrameContentUnlockedMessage(page));
                    SubFrameContentHost.Visibility = Visibility.Visible;
                }
                else
                {
                    // Fade in the new sub page
                    LoadingRing.Visibility = Visibility.Collapsed; // Hide the progress ring, if present
                    SubPage = subPage;
                    RootGrid.Visibility = SubFrameContentHost.Visibility = Visibility.Visible;
                }

                HostControl.Focus(FocusState.Keyboard);
                subPage.IsHitTestVisible = true;
            }
            finally
            {
                SubFrameSemaphoreSlim.Release();
            }
        }

        // Fades away the currently displayed sub page
        private async void CloseSubFramePage()
        {
            await SubFrameSemaphoreSlim.WaitAsync();
            try
            {
                if (!(SubPage is UserControl page)) return;

                page.IsHitTestVisible = false;
                RootGrid.Visibility = SubFrameContentHost.Visibility = Visibility.Collapsed;
                await Task.Delay(600); // Time for the animations to complete
                SubPage = null;
                Messenger.Default.Send(new SubFrameContentUnlockedMessage(page));
            }
            finally
            {
                SubFrameSemaphoreSlim.Release();
            }
        }

        // Handles the software back button
        private void SubFrameControl_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (SubPage != null) e.Handled = true; // This needs to be synchronous
            CloseSubFramePage();
        }

        #endregion

        /// <summary>
        /// Gets or sets the content of this sub frame page host
        /// </summary>
        public object SubPage
        {
            get => HostControl.Content;
            set
            {
                HostControl.Content = value;
                UpdateLayout(new Size(ActualWidth, ActualHeight));
            }
        }

        #region Host UI management

        // The minimum window width for the expanded state
        private const double ExpandedStateMinimumWidth = 880;

        // The minimum window height for the expanded state
        private const double ExpandedStateMinimumHeight = 720;

        // The margin distance allowed for constrained content to be extended over its desired size
        private const double MinimumConstrainedMarginThreshold = 48;

        // Adjusts the UI when the window is resized
        private void UpdateLayout(Size size)
        {
            // Actually updates the layout in both directions
            void UpdateLayout(double width, double height)
            {
                // Setup
                RootGrid.BorderThickness = new Thickness();
                Thickness contentBorderThickness = new Thickness(1);

                // Adjust the content width
                double targetWidth;
                if (double.IsNaN(width))
                {
                    targetWidth = double.PositiveInfinity;
                    contentBorderThickness = new Thickness(0);
                }
                else targetWidth = width;

                // Adjust the content height
                if (double.IsNaN(height))
                {
                    ContentGrid.MaxHeight = double.PositiveInfinity;

                    // Visual state update
                    if (targetWidth > size.Width - 48 * 2)
                    {
                        ContentGrid.MaxWidth = targetWidth;
                        VisualStateManager.GoToState(this, "RightBackButton", false);
                    }
                    else
                    {
                        ContentGrid.MaxWidth = targetWidth + 48;
                        VisualStateManager.GoToState(this, "TopBackButton", false);
                    }
                }
                else
                {
                    ContentGrid.MaxHeight = height + 48;
                    ContentGrid.MaxWidth = targetWidth;
                    VisualStateManager.GoToState(this, "TopBackButton", false);
                }

                // Final setup
                ContentBorder.BorderThickness = contentBorderThickness;
                ContentBorder.CornerRadius = new CornerRadius(contentBorderThickness == new Thickness(1) ? 1 : 0);
            }

            // Updates the layout according to the current type of content
            switch (SubPage)
            {
                // A constrained content, with a given maximum size
                case IConstrainedSubPage constrained:
                    {
                        double maxHeight = constrained.MaxExpandedHeight;
                        double maxWidth = constrained.MaxExpandedWidth;

                        UpdateLayout(
                            maxWidth + MinimumConstrainedMarginThreshold * 2 >= size.Width ? double.NaN : maxWidth,
                            maxHeight + MinimumConstrainedMarginThreshold * 2 >= size.Height ? double.NaN : maxHeight);
                        break;
                    }

                case IFullscreenSubPage fullscreen:
                    {
                        UpdateLayout(double.NaN, double.NaN);
                        break;
                    }

                // Any content, that toggles between almost full screen and the compact state
                case object _:
                    {
                        UpdateLayout(
                            ExpandedStateMinimumWidth >= size.Width ? double.NaN : size.Width - 160,
                            ExpandedStateMinimumHeight >= size.Height ? double.NaN : size.Height - 160);
                        break;
                    }
            }

            // Additional UI tweaks
            if (SubPage is IAdaptiveSubPage adaptive)
                adaptive.IsFullHeight = double.IsPositiveInfinity(ContentGrid.MaxHeight);

            if (SubPage is IFullscreenSubPage fullSub && !fullSub.Hideable)
                CloseButton.Visibility = Visibility.Collapsed;
            else
                CloseButton.Visibility = Visibility.Visible;


            if (SubPage is ITransparentSubPage transparentSubPage)
            {
                var transparentBackground = new SolidColorBrush(Colors.Transparent);

                BackgroundBorder.Background = transparentBackground;

                if (transparentSubPage.Dimmed)
                {
                    transparentBackground = new SolidColorBrush(Color.FromArgb(175, 0, 0, 0));
                }

                ContentBorder.Background = transparentBackground;
            }
            else
            {
                BackgroundBorder.Background = App.Current.Resources["SubFrameHostBackgroundBrush"] as Brush;
                ContentBorder.Background = App.Current.Resources["DarkBG"] as Brush;
            }
        }

        // Sends a request to close the current sub frame page
        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().GoBack();

        #endregion
    }
}
