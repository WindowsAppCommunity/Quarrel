// SubPage frame inspired by created by Sergio Pedri for BrainF*ck and Legere
// View Code in BrainF*ck
// https://github.com/Sergio0694/Brainf_ckSharp/blob/master/src/Brainf_ckSharp.Uwp/Controls/SubPages/Host/SubPageHost.xaml.cs

using Nito.AsyncEx;
using Quarrel.SubPages.Interfaces;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Host
{
    /// <summary>
    /// An adaptive host for displaying custom controls
    /// </summary>
    public sealed partial class SubPageHost : UserControl
    {
        /// <summary>
        /// Synchronization lock to avoid race conditions for user requests
        /// </summary>
        private readonly AsyncLock SubFrameLock = new();

        /// <summary>
        /// The minimum window width for the expanded state
        /// </summary>
        private const double ExpandedStateMinimumWidth = 880;

        /// <summary>
        /// The minimum window height for the expanded state
        /// </summary>
        private const double ExpandedStateMinimumHeight = 720;

        /// <summary>
        /// The margin distance allowed for constrained content to be extended over its desired size
        /// </summary>
        private const double MinimumConstrainedMarginThreshold = 48;

        /// <summary>
        /// Indicates whether or not the title bar in the current application is visible or collapsed (when in tablet mode)
        /// </summary>
        private bool _IsTitleBarVisible;

        /// <summary>
        /// Creates a new <see cref="SubPageHost"/> instance
        /// </summary>
        public SubPageHost()
        {
            this.InitializeComponent();
            this.RootGrid.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().BackRequested += SubFrameControl_BackRequested;

            // Other sub page settings
            CoreApplicationViewTitleBar titleBar = CoreApplication.GetCurrentView().TitleBar;
            _IsTitleBarVisible = titleBar.IsVisible;
            titleBar.IsVisibleChanged += (s, e) =>
            {
                _IsTitleBarVisible = s.IsVisible;
                UpdateLayout(new Size(ActualWidth, ActualHeight));
            };
        }

        /// <summary>
        /// Gets or sets the content of this sub frame page host
        /// </summary>
        private object? SubPage
        {
            get => HostControl.Content;
            set
            {
                if (value == null && HostControl.Content != null) SizeChanged -= SubFrameHost_SizeChanged;
                else if (value != null && HostControl.Content == null) SizeChanged += SubFrameHost_SizeChanged;
                HostControl.Content = value;
                UpdateLayout(new Size(ActualWidth, ActualHeight));
                SizeChanged += SubFrameHost_SizeChanged;
            }
        }

        /// <summary>
        /// Displays a page in the popup frame
        /// </summary>
        /// <param name="subPage">The page to display</param>
        public async void DisplaySubFramePage(UserControl subPage)
        {
            using (await SubFrameLock.LockAsync())
            {
                // Fade out the current content, if present
                if (SubPage is UserControl page)
                {
                    page.IsHitTestVisible = false;
                    SubFrameContentHost.Visibility = Visibility.Collapsed;
                    await Task.Delay(400); // Time for the animations to complete
                    SubPage = subPage;
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
        }

        /// <summary>
        /// Fades away the currently displayed sub page
        /// </summary>
        public async void CloseSubFramePage()
        {
            using (await SubFrameLock.LockAsync())
            {
                if (!(SubPage is UserControl page)) return;

                page.IsHitTestVisible = false;
                RootGrid.Visibility = SubFrameContentHost.Visibility = Visibility.Collapsed;
                await Task.Delay(600); // Time for the animations to complete
                SubPage = null;
            }
        }

        // Handles the software back button
        private void SubFrameControl_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (SubPage != null) e.Handled = true; // This needs to be synchronous

            CloseSubFramePage();
        }

        // Executes a UI refresh when the root size changes
        private void SubFrameHost_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateLayout(e.NewSize);

        /// <summary>
        /// Adjusts the UI when the window is resized
        /// </summary>
        /// <param name="size">The new available size for the control</param>
        private void UpdateLayout(Size size)
        {
            // The local function that updates the layout in both directions
            void UpdateLayout(double width, double height)
            {
                // Setup
                RootGrid.BorderThickness = new Thickness();
                Thickness contentBorderThickness = new(1);

                // Adjust the content width
                double targetWidth;
                if (double.IsNaN(width))
                {
                    targetWidth = double.PositiveInfinity;
                    contentBorderThickness.Left = contentBorderThickness.Right = 0;
                }
                else targetWidth = width;

                // Adjust the content height
                if (double.IsNaN(height))
                {
                    ContentGrid.MaxHeight = double.PositiveInfinity;
                    contentBorderThickness.Top = contentBorderThickness.Bottom = 0;

                    // Visual state update
                    if (targetWidth > size.Width - 48 * 2)
                    {
                        ContentGrid.MaxWidth = targetWidth;
                        VisualStateManager.GoToState(this, nameof(TopBackButton), false);
                        CloseButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ContentGrid.MaxWidth = targetWidth + 48;
                        VisualStateManager.GoToState(this, nameof(LeftBackButton), false);
                        CloseButton.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    ContentGrid.MaxHeight = height + 48;
                    ContentGrid.MaxWidth = targetWidth;
                    VisualStateManager.GoToState(this, nameof(TopBackButton), false);
                    CloseButton.Visibility = Visibility.Visible;
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

                        // Disable the fullscreen mode if requested, and if there's enough space
                        if (size.Height >= 600 && size.Width >= 540 &&
                            double.IsInfinity(constrained.MaxExpandedHeight) && double.IsInfinity(constrained.MaxExpandedWidth))
                        {
                            maxHeight = size.Height - 160;
                            maxWidth = size.Width - 160;
                        }

                        UpdateLayout(
                            maxWidth + MinimumConstrainedMarginThreshold * 2 >= size.Width ? double.NaN : maxWidth,
                            maxHeight + MinimumConstrainedMarginThreshold * 2 >= size.Height ? double.NaN : maxHeight);
                        break;
                    }

                // Any content, that toggles between almost full screen and the compact state
                case { } _:
                    {
                        UpdateLayout(
                            ExpandedStateMinimumWidth >= size.Width ? double.NaN : size.Width - 160,
                            ExpandedStateMinimumHeight >= size.Height ? double.NaN : size.Height - 160);
                        break;
                    }
            }

            // Additional UI tweaks
            if (SubPage is IAdaptiveSubPage adaptive)
            {
                adaptive.IsFullHeight = double.IsPositiveInfinity(ContentGrid.MaxHeight) && _IsTitleBarVisible;
            }
        }
    }
}
