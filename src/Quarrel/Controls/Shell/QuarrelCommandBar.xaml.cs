using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Models.Bindables;
using Quarrel.SubPages.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Navigation;
using Quarrel.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
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

        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        public bool ShowHamburger { get; set; }

        private GuildChannel GuildChannel { get => ViewModel.Channel != null ? ViewModel.Channel.Model as GuildChannel : null; }

        private string ChannelTopic { get => GuildChannel != null ? GuildChannel.Topic : ""; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var layoutRoot = GetTemplateChild("LayoutRoot") as Grid;
            if (layoutRoot != null)
            {
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, new OpenDownCommandBarVisualStateManager());
            }
        }

        public event EventHandler HamburgerClicked;
        private void ToggleSplitView(object sender, RoutedEventArgs e)
        {
            HamburgerClicked(this, null);
        }

        public event EventHandler MemberListButtonClicked;
        private void ToggleMemberPane(object sender, RoutedEventArgs e)
        {
            MemberListButtonClicked(this, null);
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("SettingsPage");
        }

        private void OpenAbout(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("AboutPage");
        }

        private void OpenWhatsNew(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("WhatsNewPage");
        }

        private void ChannelNameTapped(object sender, TappedRoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("TopicPage", ViewModel.Channel);
        }
    }

    public class OpenDownCommandBarVisualStateManager : VisualStateManager
    {
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
