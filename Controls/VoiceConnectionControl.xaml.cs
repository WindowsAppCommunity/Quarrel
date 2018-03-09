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
            App.UpdateLocalMute(true);
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
            AudioManager.DisposeAudioGraphs();
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
            if (Minimode.IsChecked == true)
            {
                Minimode.IsChecked = false;
                App.ToggleCOMode();
            }
            App.NavigateToGuild(guildid);
        }

        private void SetupVisualizer()
        {
            smoother1 = new Smoother(4, 6);
            smoother2 = new Smoother(4, 12);
            smoother3 = new Smoother(4, 14);
            smoother4 = new Smoother(4, 14);
            smoother5 = new Smoother(4, 15);
            smoother6 = new Smoother(4, 16);
            smoother7 = new Smoother(4, 16);
            smoother8 = new Smoother(4, 15);
            smoother9 = new Smoother(4, 14);
            averageSmoother = new Smoother(1000, 100);
            Blurple = (Color)App.Current.Resources["BlurpleColor"];
            TransparentBlurple = (Color)App.Current.Resources["BlurpleColorTransparent"];
        }
        Smoother smoother1;
        Smoother smoother2;
        Smoother smoother3;
        Smoother smoother4;
        Smoother smoother5;
        Smoother smoother6;
        Smoother smoother7;
        Smoother smoother8;
        Smoother smoother9;
        Smoother averageSmoother;
        public class Smoother
        {
            /// <summary>
            /// Initialize a new smart continuous average algorithm, or SCAA (I made that name up)
            /// </summary>
            /// <param name="SmoothTime">The smoothing window in *10ms</param>
            /// /// <param name="multiplier">The opacity multiplier (5 by default)</param>
            public Smoother(int smoothTime, float multiplier = 5, double smoothnessThresholdUp = 1, double smoothnessThresholdDown = 1, float smoothLimit = 1f)
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
                //if ((input - PreviousVal) < SmoothingThresholdUp && (PreviousVal - input) < SmoothingThresholdDown && input < SmoothLimit)
                    input = (((PreviousVal * SmoothTime) + input) / (SmoothTime + 1));

                PreviousVal = input;
                return input * Multiplier;
            }
        }

        Color Blurple;
        Color TransparentBlurple;
        float height = 47;
        float HalfPoint;
        float Point0 = 0;
        float Point1;
        float Point2;
        float Point3;
        float Point4;
        float Point5;
        float Point6;
        float Point7;
        float Point8;

        Vector2 GetC1(Vector2 input)
        {
            return new Vector2(input.X+HalfPoint, input.Y);
        }
        Vector2 GetC2(Vector2 input)
        {
            return new Vector2(input.X-HalfPoint, input.Y);
        }
        float average = 0;
        float Adjust(float input)
        {
            float multiplier = 1 + ((1 - average) * 4);
            if (multiplier < 1) multiplier = 1;
            return input* multiplier;
        }
        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                cpb.BeginFigure(0, height);
                average = averageSmoother.Smooth(AudioManager.AudioAverage);
                Vector2 p0 = new Vector2(Point0, height - Adjust(smoother1.Smooth(AudioManager.AudioSpec1)) * height);
                Vector2 p1 = new Vector2(Point1, height - Adjust(smoother2.Smooth(AudioManager.AudioSpec2)) * height);
                Vector2 p2 = new Vector2(Point2, height - Adjust(smoother3.Smooth(AudioManager.AudioSpec3)) * height);
                Vector2 p3 = new Vector2(Point3, height - Adjust(smoother4.Smooth(AudioManager.AudioSpec4)) * height);
                Vector2 p4 = new Vector2(Point4, height - Adjust(smoother5.Smooth(AudioManager.AudioSpec5)) * height);
                Vector2 p5 = new Vector2(Point5, height - Adjust(smoother6.Smooth(AudioManager.AudioSpec6)) * height);
                Vector2 p6 = new Vector2(Point6, height - Adjust(smoother7.Smooth(AudioManager.AudioSpec7)) * height);
                Vector2 p7 = new Vector2(Point7, height - Adjust(smoother8.Smooth(AudioManager.AudioSpec8)) * height);
                Vector2 p8 = new Vector2(Point8, height - Adjust(smoother9.Smooth(AudioManager.AudioSpec9)) * height);



                cpb.AddLine(p0);
                cpb.AddCubicBezier(GetC1(p0), GetC2(p1), p1);
                cpb.AddCubicBezier(GetC1(p1), GetC2(p2), p2);
                cpb.AddCubicBezier(GetC1(p2), GetC2(p3), p3);
                cpb.AddCubicBezier(GetC1(p3), GetC2(p4), p4);
                cpb.AddCubicBezier(GetC1(p4), GetC2(p5), p5);
                cpb.AddCubicBezier(GetC1(p5), GetC2(p6), p6);
                cpb.AddCubicBezier(GetC1(p6), GetC2(p7), p7);
                cpb.AddCubicBezier(GetC1(p7), GetC2(p8), p8);
                cpb.AddLine(new Vector2(p8.X, height));
                
      
                cpb.EndFigure(CanvasFigureLoop.Closed);
                CanvasLinearGradientBrush gradient = new CanvasLinearGradientBrush(sender, TransparentBlurple, Blurple)
                {
                    EndPoint = new Vector2(0, height+48),
                    StartPoint = new Vector2(0, -12)
                };
                var path = CanvasGeometry.CreatePath(cpb);
                //args.DrawingSession.DrawGeometry(path, Blurple, 1);
                args.DrawingSession.FillGeometry(path, gradient);
            }
        }

        private void CanvasAnimatedControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            height = (float)e.NewSize.Height - 1;
            float Segment = (float)e.NewSize.Width / 8;
            Point1 = Segment;
            HalfPoint = Segment / 2;
            Point2 = Segment * 2;
            Point3 = Segment * 3;
            Point4 = Segment * 4;
            Point5 = Segment * 5;
            Point6 = Segment * 6;
            Point7 = Segment * 7;
            Point8 = Segment * 8;
        }
    }
}
