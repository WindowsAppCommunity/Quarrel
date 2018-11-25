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

using Discord_UWP.Managers;
using Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;


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
            App.UpdateVoiceStateHandler += App_UpdateVoiceStateHandler;
            //Loaded += fftInitialize;
            //Unloaded += fftDipose;
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
                MainGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                MainGrid.VerticalAlignment = VerticalAlignment.Stretch;
                MainGrid.Background = (App.Current.Resources["AcrylicUserBackgroundDarker"] as Brush);
                ShowChannel.Begin();
                if (App.ShowAds)
                {
                    FullScreenAdBanner.Visibility = Visibility.Visible;
                }
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
            if (VoiceManager.lockMute)
            {
                Mute.IsEnabled = false;
                Mute.IsChecked = true;
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            guildid = "";
            channelid = "";
            App.ConnectToVoice(null, null, "", "");
            AudioManager.LightDisposeAudioGraphs();
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
        }

        private void MiniView_Click(object sender, RoutedEventArgs e)
        {
            App.ToggleCOMode();
        }

        private void Deafen_Click(object sender, RoutedEventArgs e)
        {
            App.UpdateLocalDeaf(!LocalModels.LocalState.VoiceState.SelfDeaf);
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
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
            if (Minimode.IsChecked == true)
            {
                Minimode.IsChecked = false;
                App.ToggleCOMode();
            }
            App.NavigateToGuild(guildid);
        }

        private async void OpenAudioCaptrueFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideInputDevice;
                menu.Items.Add(defaultflyoutItem);

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

                //MenuFlyoutSeparator separator = new MenuFlyoutSeparator();
                //menu.Items.Add(separator);

                //var odevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioRender);
                //foreach (var device in odevices)
                //{
                //    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                //    {
                //        Text = device.Name,
                //        Tag = device.Id,
                //        IsEnabled = device.IsEnabled
                //    };
                //    flyoutItem.Click += OverrideInputDevice;
                //    menu.Items.Add(flyoutItem);
                //}

                menu.ShowAt(this, e.GetPosition(this));
            }
        }

        private async void OpenAudioCaptureFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                MenuFlyout menu = new MenuFlyout();
                menu.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];

                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideInputDevice;
                menu.Items.Add(defaultflyoutItem);

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

                //MenuFlyoutSeparator separator = new MenuFlyoutSeparator();
                //menu.Items.Add(separator);

                //var odevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioRender);
                //foreach (var device in odevices)
                //{
                //    MenuFlyoutItem flyoutItem = new MenuFlyoutItem()
                //    {
                //        Text = device.Name,
                //        Tag = device.Id,
                //        IsEnabled = device.IsEnabled
                //    };
                //    flyoutItem.Click += OverrideInputDevice;
                //    menu.Items.Add(flyoutItem);
                //}

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

                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideOutputDevice;
                menu.Items.Add(defaultflyoutItem);

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

                MenuFlyoutItem defaultflyoutItem = new MenuFlyoutItem()
                {
                    Text = "Default",
                    Tag = "Default"
                };
                defaultflyoutItem.Click += OverrideOutputDevice;
                menu.Items.Add(defaultflyoutItem);

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
                menu.ShowAt(this, e.GetPosition(this));
            }
        }

        private void OverrideOutputDevice(object sender, RoutedEventArgs e)
        {
            AudioManager.UpdateOutputDeviceID((sender as MenuFlyoutItem).Tag.ToString());
        }

        private void OverrideInputDevice(object sender, RoutedEventArgs e)
        {
            AudioManager.UpdateInputDeviceID((sender as MenuFlyoutItem).Tag.ToString());
        }

        public void Dispose()
        {
            App.VoiceConnectHandler -= App_VoiceConnectHandler;
            App.ToggleCOModeHandler -= App_ToggleCOModeHandler;
            App.UpdateVoiceStateHandler -= App_UpdateVoiceStateHandler;
            App.NavigateToGuildHandler -= App_NavigateToGuildHandler;
        }
    }
}
