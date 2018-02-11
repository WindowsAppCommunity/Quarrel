using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class VoiceConnectionControl : UserControl
    {
        public VoiceConnectionControl()
        {
            this.InitializeComponent();
            if (!(App.IsDesktop || App.IsTablet))
            {
                miniViewColumn.Width = new GridLength(0);
            }
            App.VoiceConnectHandler += App_VoiceConnectHandler;
            App.ToggleCOModeHandler += App_ToggleCOModeHandler;
        }

        public bool FullScreen
        {
            get { return (bool)GetValue(FullscreenProperty); }
            set { SetValue(FullscreenProperty, value); }
        }
        public static readonly DependencyProperty FullscreenProperty = DependencyProperty.Register(
            nameof(FullScreen),
            typeof(bool),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VoiceConnectionControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == FullscreenProperty)
            {
                MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                MainGrid.VerticalAlignment = VerticalAlignment.Stretch;
                MainGrid.Background = (App.Current.Resources["AcrylicUserBackgroundDarker"] as Brush);
            }
        }

        private void App_ToggleCOModeHandler(object sender, EventArgs e)
        {
            Minimode.IsChecked = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default;
        }

        public string guildid = "";
        public string channelid = "";
        private void App_NavigateToGuildHandler(object sender, App.GuildNavigationArgs e)
        {
            if (e.GuildId == guildid && ChannelGrid.Visibility != Visibility.Collapsed)
                HideChannel.Begin();
            else if (ChannelGrid.Visibility == Visibility.Collapsed)
                ShowChannel.Begin();
        }

        public void Show()
        {
            ShowContent.Begin();
        }
        public void Hide()
        {
            HideContent.Begin();
        }
        private void App_VoiceConnectHandler(object sender, App.VoiceConnectArgs e)
        {
            App.NavigateToGuildHandler += App_NavigateToGuildHandler;
            guildid = e.GuildId;
            channelid = e.ChannelId;
            ChannelName.Text = e.ChannelName;
            GuildName.Text = e.GuildName;
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            guildid = "";
            channelid = "";
            App.ConnectToVoice(null, null, "","");
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
        }

        private async void MiniView_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleCOMode();
        }

        private void Deafen_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Toggle local deafen
            AudioManager.ChangeDeafStatus(Deafen.IsChecked.Value);
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Toggle local mute
            App.UpdateLocalMute(!LocalModels.LocalState.VoiceState.SelfMute);
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AudioManager.ChangeVolume(e.NewValue/100);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Expanded.Visibility == Visibility.Visible)
                HideExpanded.Begin();
            else
                ShowExpanded.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuild(guildid);
        }
    }
}
