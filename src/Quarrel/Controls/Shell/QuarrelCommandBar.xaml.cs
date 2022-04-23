// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Messages.Panel;
using Quarrel.Services.Windows;
using Quarrel.ViewModels.Panels;
using Quarrel.ViewModels.SubPages.DiscordStatus;
using Quarrel.ViewModels.SubPages.Meta;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class QuarrelCommandBar : UserControl
    {
        private readonly IWindowService _windowService;
        private readonly IMessenger _messenger;

        private readonly DependencyProperty ShowHamburgerButtonProperty =
            DependencyProperty.Register(nameof(ShowHamburgerButton), typeof(bool), typeof(QuarrelCommandBar), new PropertyMetadata(true, OnShowHamburgerButtonPropertyChanged));

        private readonly DependencyProperty ShowToggleMemberButtonProperty =
            DependencyProperty.Register(nameof(ShowToggleMemberButton), typeof(bool), typeof(QuarrelCommandBar), new PropertyMetadata(true, OnToggleMemberButtonPropertyChanged));

        public QuarrelCommandBar()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<ChannelsViewModel>();

            _messenger = App.Current.Services.GetRequiredService<IMessenger>();
            _windowService = App.Current.Services.GetRequiredService<IWindowService>();
        }

        public ChannelsViewModel ViewModel => (ChannelsViewModel)DataContext;

        public bool ShowHamburgerButton
        {
            get => (bool)GetValue(ShowHamburgerButtonProperty);
            set => SetValue(ShowHamburgerButtonProperty, value);
        }

        public bool ShowToggleMemberButton
        {
            get => (bool)GetValue(ShowToggleMemberButtonProperty);
            set => SetValue(ShowToggleMemberButtonProperty, value);
        }

        private void ToggleMemberList(object sender, RoutedEventArgs e)
            => _messenger.Send(new TogglePanelMessage(PanelSide.Right, PanelState.Toggle));

        private void HamburgerClicked(object sender, RoutedEventArgs e)
            => _messenger.Send(new TogglePanelMessage(PanelSide.Left, PanelState.Toggle));

        private void GoToDiscordStatus(object sender, RoutedEventArgs e)
            => _messenger.Send(new NavigateToSubPageMessage(typeof(DiscordStatusViewModel)));

        private void GoToAbout(object sender, RoutedEventArgs e)
            => _messenger.Send(new NavigateToSubPageMessage(typeof(AboutPageViewModel)));

        private static void OnShowHamburgerButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            QuarrelCommandBar commandBar = (QuarrelCommandBar)d;
            bool newValue = (bool)args.NewValue;
            commandBar.HamburgerButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnToggleMemberButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            QuarrelCommandBar commandBar = (QuarrelCommandBar)d;
            bool newValue = (bool)args.NewValue;
            commandBar.ToggleMembersBTN.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OpenInNewWindow(object sender, RoutedEventArgs e)
        {
            _windowService.OpenSecondaryWindow();
        }
    }
}
