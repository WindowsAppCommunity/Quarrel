using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
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
using Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;
using Quarrel.Managers;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class VoiceConnectionControl : UserControl
    {
        public VoiceConnectionControl()
        {
            this.InitializeComponent();
            
            // Hide and disable PIP icon when not on Desktop or Tablet
            if (!(App.IsDesktop || App.IsTablet))
            {
                miniViewColumn.Width = new GridLength(0);
                Minimode.IsEnabled = false;
            }

            // Setup event
            App.VoiceConnectHandler += App_VoiceConnectHandler;
            App.ToggleCOModeHandler += App_ToggleCOModeHandler;
            App.UpdateVoiceStateHandler += App_UpdateVoiceStateHandler;
        }

        private async void App_UpdateVoiceStateHandler(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     Deafen.IsChecked = LocalModels.LocalState.VoiceState.SelfDeaf;
                     Mute.IsChecked = LocalModels.LocalState.VoiceState.SelfMute;
                 });
        }

        /// <summary>
        /// (Depricated)
        /// </summary>
        public bool FullScreen
        {
            get => (bool)GetValue(FullscreenProperty);
            set => SetValue(FullscreenProperty, value);
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
                // Stretch Control
                MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                MainGrid.VerticalAlignment = VerticalAlignment.Stretch;

                // Darken background
                MainGrid.Background = (App.Current.Resources["AcrylicUserBackgroundDarker"] as Brush);

                // Show current channel promp 
                ShowChannel.Begin();

                // If ads aren't disabled
                if (App.ShowAds)
                {
                    // Show ad
                    FullScreenAdBanner.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Minimode status updated
        /// </summary>
        private void App_ToggleCOModeHandler(object sender, EventArgs e)
        {
            // Set minimode is checked to the current view status
            Minimode.IsChecked = ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default;
        }

        /// <summary>
        /// Id of guild for voice channel
        /// </summary>
        public string guildid = "";

        /// <summary>
        /// Id of voice channel
        /// </summary>
        public string channelid = "";

        /// <summary>
        /// Guild navigated
        /// </summary>
        private void App_NavigateToGuildHandler(object sender, App.GuildNavigationArgs e)
        {
            // If the new guild contains the voice channel, hide navigate button
            if (e.GuildId == guildid && ChannelGrid.Visibility != Visibility.Collapsed)
                HideChannel.Begin();

            // If the new guild does not contain the voice channel, show navigation button
            else if (ChannelGrid.Visibility == Visibility.Collapsed)
                ShowChannel.Begin();
        }

        /// <summary>
        /// Show FFT
        /// </summary>
        public void Show()
        {
            ShowContent.Begin();
        }

        /// <summary>
        /// Hide FFT
        /// </summary>
        public void Hide()
        {
            HideContent.Begin();
        }

        /// <summary>
        /// Voice channel connected to
        /// </summary>
        private void App_VoiceConnectHandler(object sender, App.VoiceConnectArgs e)
        {
            // Add navigated prompt
            App.NavigateToGuildHandler += App_NavigateToGuildHandler;

            // Update ids
            guildid = e.GuildId;
            channelid = e.ChannelId;

            // Update names
            ChannelName.Text = e.ChannelName;
            GuildName.Text = e.GuildName;

            // If mute is locked
            if (VoiceManager.lockMute)
            {
                Mute.IsEnabled = false;
                Mute.IsChecked = true;
            }
        }

        /// <summary>
        /// Prompt disconnect
        /// </summary>
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            // Dispose ids
            guildid = "";
            channelid = "";

            // Disconnect
            App.ConnectToVoice(null, null, "", "");

            // Prompt potential AudioGraphs disposal
            AudioManager.LightDisposeAudioGraphs();

            // Dispose navigate event
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
        }

        /// <summary>
        /// Toggle PIP
        /// </summary>
        private void MiniView_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleCOMode();
        }

        /// <summary>
        /// Local deafen user
        /// </summary>
        private void Deafen_Click(object sender, RoutedEventArgs e)
        {
            App.UpdateLocalDeaf(!LocalModels.LocalState.VoiceState.SelfDeaf);
        }

        /// <summary>
        /// Local mute user
        /// </summary>
        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            App.UpdateLocalMute(!LocalModels.LocalState.VoiceState.SelfMute);
        }

        /// <summary>
        /// Adjust volume
        /// </summary>
        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AudioManager.ChangeVolume(e.NewValue/100);
        }

        /// <summary>
        /// Toggle Expanded control
        /// </summary>
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide
            if (Expanded.Visibility == Visibility.Visible)
                HideExpanded.Begin();
            // Show
            else
                ShowExpanded.Begin();
        }

        /// <summary>
        /// Navigate to guild by <see cref="guildid"/>
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Minimode.IsChecked == true)
            {
                Minimode.IsChecked = false;
                App.ToggleCOMode();
            }
            App.NavigateToGuild(guildid);
        }

        /// <summary>
        /// Open Input device select flyout
        /// </summary>
        private async void OpenAudioCaptrueFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                // Add default value 
                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideInputDevice;
                menu.Items.Add(defaultflyoutItem);

                // Add devices to device list
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioCapture);
                foreach (var device in devices)
                {
                    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                    {
                        Text = device.Name,
                        Tag = device.Id,
                        IsEnabled = device.IsEnabled
                    };
                    flyoutItem.Click += OverrideInputDevice;
                    menu.Items.Add(flyoutItem);
                }

                // Show menu
                menu.ShowAt(this, e.GetPosition(this));
            }
        }

        private async void OpenAudioCaptureFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                // Add default value
                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideInputDevice;
                menu.Items.Add(defaultflyoutItem);

                // Add devices to device list
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioCapture);
                foreach (var device in devices)
                {
                    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                    {
                        Text = device.Name,
                        Tag = device.Id,
                        IsEnabled = device.IsEnabled
                    };
                    flyoutItem.Click += OverrideInputDevice;
                    menu.Items.Add(flyoutItem);
                }

                // Show menu
                menu.ShowAt(this, e.GetPosition(this));
            }
            e.Handled = true;
        }

        private async void OpenAudioRenderFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                // Add default value
                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideOutputDevice;
                menu.Items.Add(defaultflyoutItem);

                // Add devices to device list
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioRender);
                foreach (var device in devices)
                {
                    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                    {
                        Text = device.Name,
                        Tag = device.Id,
                        IsEnabled = device.IsEnabled
                    };
                    flyoutItem.Click += OverrideOutputDevice;
                    menu.Items.Add(flyoutItem);
                }

                // Show menu
                menu.ShowAt(this, e.GetPosition(this));
            }
        }

        private async void OpenAudioRenderFlyout(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                // Add default value
                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideOutputDevice;
                menu.Items.Add(defaultflyoutItem);

                // Add devices to device list
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioRender);
                foreach (var device in devices)
                {
                    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                    {
                        Text = device.Name,
                        Tag = device.Id,
                        IsEnabled = device.IsEnabled
                    };
                    flyoutItem.Click += OverrideOutputDevice;
                    menu.Items.Add(flyoutItem);
                }
                
                // Show menu
                menu.ShowAt(this, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Override Output device by device id
        /// </summary>
        private void OverrideOutputDevice(object sender, RoutedEventArgs e)
        {
            AudioManager.UpdateOutputDeviceID((sender as MenuFlyoutItem).Tag.ToString());
        }

        /// <summary>
        ///  Override Input device by device id
        /// </summary>
        private void OverrideInputDevice(object sender, RoutedEventArgs e)
        {
            AudioManager.UpdateInputDeviceID((sender as MenuFlyoutItem).Tag.ToString());
        }

        /// <summary>
        /// Dipose of VoiceConnectionControl
        /// </summary>
        public void Dispose()
        {
            App.VoiceConnectHandler -= App_VoiceConnectHandler;
            App.ToggleCOModeHandler -= App_ToggleCOModeHandler;
            App.UpdateVoiceStateHandler -= App_UpdateVoiceStateHandler;
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
        }
    }
}
