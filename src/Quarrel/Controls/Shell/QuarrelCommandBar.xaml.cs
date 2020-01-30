using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Navigation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    /// <summary>
    /// Custom CommandBar instance used in shell
    /// </summary>
    public sealed partial class QuarrelCommandBar : CommandBar
    {
        public QuarrelCommandBar()
        {
            this.InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Access app's main data
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        #region Events
        
        /// <summary>
        /// Invoked when Hamburger button is clicked
        /// </summary>
        public event EventHandler HamburgerClicked;

        /// <summary>
        /// Invoked when MemberListToggle button is clicked
        /// </summary>
        public event EventHandler MemberListButtonClicked;

        #endregion

        #region Methods

        /// <summary>
        /// Changes the VisualStateManager to confirm open down
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
            {
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, new OpenDownCommandBarVisualStateManager());
            }
        }

        /// <summary>
        /// Invokes the HumburgerClicked event
        /// </summary>
        private void InvokeHumburgerClick(object sender, RoutedEventArgs e)
        {
            HamburgerClicked(this, null);
        }

        /// <summary>
        /// Invokes the MemberListButtonClicked event
        /// </summary>
        private void InvokeMemberListToggleClick(object sender, RoutedEventArgs e)
        {
            MemberListButtonClicked(this, null);
        }

        /// <summary>
        /// Opens Channel TopicPage for current channel
        /// </summary>
        private void ChannelNameTapped(object sender, TappedRoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("TopicPage", ViewModel.CurrentChannel);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the Hamburger button should be shown by this CommandBar
        /// </summary>
        public bool ShowHamburger { get; set; }

        /// <summary>
        /// Gets Current channel as GuildChanne;
        /// </summary>
        private GuildChannel GuildChannel { get => ViewModel.CurrentChannel != null ? ViewModel.CurrentChannel.Model as GuildChannel : null; }

        /// <summary>
        /// Gets the topic of the current channel (if GuildChannel)
        /// </summary>
        private string ChannelTopic { get => GuildChannel != null ? GuildChannel.Topic : ""; }

        #endregion
    }

    /// <summary>
    /// VisualStateManager for CommandBar that guarentees opening down
    /// </summary>
    public class OpenDownCommandBarVisualStateManager : VisualStateManager
    {
        /// <inheritdoc/>
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            //replace OpenUp state change with OpenDown one and continue as normal
            if (!string.IsNullOrWhiteSpace(stateName) && stateName.EndsWith("OpenUp"))
            {
                stateName = stateName.Substring(0, stateName.Length - 6) + "OpenDown";
            }
            return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
        }
    }
}
