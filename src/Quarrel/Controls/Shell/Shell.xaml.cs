using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages.Navigation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    /// <summary>
    /// Main shell used for all Quarrel content
    /// </summary>
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            try
            {
                this.InitializeComponent();

                // Setup SideDrawer
                ContentContainer.SetupInteraction();

                Messenger.Default.Register<GuildNavigateMessage>(this, m =>
                {
                    ContentContainer.OpenLeft();
                });

                Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
                {
                    ContentContainer.CloseLeft();
                });
            }
            catch (Exception ex)
            {
                var logger = App.ServiceProvider.GetService<ILogger<Shell>>();

                do
                {
                    logger.LogError(new EventId(), ex, "Error constructing Shell");
                    ex = ex.InnerException;
                } while (ex != null);

                throw;
            }
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Determines if View should use the large or small view QuarrelCommandBar
        /// </summary>
        private bool UseLargeCommandBar => UISize.CurrentState == Large || UISize.CurrentState == ExtraLarge;

        /// <summary>
        /// Updates bindings when UI Size changes
        /// </summary>
        private void UISize_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            this.Bindings.Update();
        }

        /// <summary>
        /// Toggles Left panes when Hamburger button is press
        /// </summary>
        private void HamburgerClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleLeft();
        }

        /// <summary>
        /// Toggles right pane when MemberListToggle button is press
        /// </summary>
        private void MemberListButtonClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleRight();
        }
    }
}
