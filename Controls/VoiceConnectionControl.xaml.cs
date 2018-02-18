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
            SetupVisualizer();
            App.VoiceConnectHandler += App_VoiceConnectHandler;
            App.ToggleCOModeHandler += App_ToggleCOModeHandler;
            App.UpdateVoiceStateHandler += App_UpdateVoiceStateHandler;
        }

        private void App_UpdateVoiceStateHandler(object sender, EventArgs e)
        {
            Deafen.IsChecked = LocalModels.LocalState.VoiceState.SelfDeaf;
            Mute.IsChecked = LocalModels.LocalState.VoiceState.SelfMute;
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
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            guildid = "";
            channelid = "";
            App.ConnectToVoice(null, null, "","");
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

        private void SetupVisualizer()
        {
            smoother1 = new Smoother(12, 5, 0.5, 0.8);
            smoother2 = new Smoother(8, 5);
            smoother3 = new Smoother(8, 6);
            smoother4 = new Smoother(8, 6);
            smoother5 = new Smoother(4, 6);
        }
        Smoother smoother1;
        Smoother smoother2;
        Smoother smoother3;
        Smoother smoother4;
        Smoother smoother5;
        public class Smoother
        {
            /// <summary>
            /// Initialize a new smart continuous average algorithm, or SCAA (I made that name up)
            /// </summary>
            /// <param name="SmoothTime">The smoothing window in *10ms</param>
            /// /// <param name="multiplier">The opacity multiplier (5 by default)</param>
            public Smoother(int smoothTime, float multiplier = 5, double smoothnessThresholdUp = 0.2, double smoothnessThresholdDown = 0.6, float smoothLimit = 0.82f)
            {
                SmoothTime = smoothTime;
                Multiplier = multiplier;
                SmoothLimit = smoothLimit / multiplier;
                SmoothingThresholdDown = smoothnessThresholdDown;
                SmoothingThresholdUp = smoothnessThresholdUp;
            }
            //This is the value above or below which the algorithm ignores smoothing and jumps to the new value
            //This is useful to give more liveliness to the visualization
            public double SmoothingThresholdUp = 1;
            public double SmoothingThresholdDown = 1;
            public float Multiplier = 0;
            public float SmoothTime = 0;
            public float PreviousVal = 0;
            public float SmoothLimit = 0.82f;
            //If the difference with the previous sample isn't too big, This function uses a simple moving average formula to smooth the value out
            public float Smooth(float input)
            {
                if ((input - PreviousVal) < SmoothingThresholdUp && (PreviousVal - input) < SmoothingThresholdDown && input < SmoothLimit)
                    input = (((PreviousVal * SmoothTime) + input) / (SmoothTime + 1));

                PreviousVal = input;
                return input * Multiplier;
            }
        }

        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            var width = 12 + 48 * AudioManager.AudioSpec1;
            Vector2 Point1 = new Vector2(0, smoother1.Smooth(AudioManager.AudioSpec1) * 64);
            Vector2 Point2 = new Vector2(57, smoother2.Smooth(AudioManager.AudioSpec2) * 64);
            Vector2 Point3 = new Vector2(114, smoother3.Smooth(AudioManager.AudioSpec3) * 64);
            Vector2 Point4 = new Vector2(171, smoother4.Smooth(AudioManager.AudioSpec4) * 64);
            Vector2 Point5 = new Vector2(228, smoother5.Smooth(AudioManager.AudioSpec5) * 64);
            args.DrawingSession.DrawLine(Point1, Point2, Colors.Red);
            args.DrawingSession.DrawLine(Point2, Point3, Colors.Red);
            args.DrawingSession.DrawLine(Point3, Point4, Colors.Red);
            args.DrawingSession.DrawLine(Point4, Point5, Colors.Red);
        }
    }
}
