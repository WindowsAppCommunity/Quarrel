// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Quarrel.Helpers.AudioProcessing;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Voice.Audio;
using Quarrel.ViewModels.Services.Voice.Audio.In;
using Quarrel.ViewModels.Services.Voice.Audio.Out;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AcrylicBrush = Microsoft.UI.Xaml.Media.AcrylicBrush;

namespace Quarrel.Controls
{
    /// <summary>
    /// Control that displays a frourier transform of a PCM audio stream.
    /// </summary>
    public sealed partial class AudioVisualizer : UserControl
    {
        /// <summary>
        /// Indicates if incoming data should be rendered.
        /// </summary>
        private bool initailized = false;

        /// <summary>
        /// Color to render polygon.
        /// </summary>
        private Color _blurple;

        /// <summary>
        /// Near the top it gradients to transparent. This needs to have the same RGB as <see cref="_blurple"/> but with 00 A.
        /// </summary>
        private Color _transparentBlurple;

        /// <summary>
        /// Height of object - 1 (will change if not 47).
        /// </summary>
        private float _height = 47;

        /// <summary>
        /// The middle offset of a data point.
        /// </summary>
        private float _halfPoint;

        private readonly float _point0 = 0;
        private float _point1 = 0;
        private float _point2 = 0;
        private float _point3 = 0;
        private float _point4 = 0;
        private float _point5 = 0;
        private float _point6 = 0;
        private float _point7 = 0;
        private float _point8 = 0;

        /// <summary>
        /// Average value of points.
        /// </summary>
        private float average = 0;

        private float _specPoint0 = 0;
        private float _specPoint1 = 0;
        private float _specPoint2 = 0;
        private float _specPoint3 = 0;
        private float _specPoint4 = 0;
        private float _specPoint5 = 0;
        private float _specPoint6 = 0;
        private float _specPoint7 = 0;
        private float _specPoint8 = 0;

        /// <summary>
        /// Average value of Spec Points.
        /// </summary>
        private float _specPointAverage = 0;

        private Smoother _smoother1;
        private Smoother _smoother2;
        private Smoother _smoother3;
        private Smoother _smoother4;
        private Smoother _smoother5;
        private Smoother _smoother6;
        private Smoother _smoother7;
        private Smoother _smoother8;
        private Smoother _smoother9;
        private Smoother _averageSmoother;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizer"/> class.
        /// </summary>
        public AudioVisualizer()
        {
            this.InitializeComponent();

            (Resources["AcrylicBlur"] as AcrylicBrush).TintLuminosityOpacity = 0;
            Loaded += FftInitialize;
            Unloaded += FftDipose;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Visualizer is displaying input or output.
        /// </summary>
        public bool Input { get; set; }

        private IAudioService BoundAudioService => Input ? (IAudioService)SimpleIoc.Default.GetInstance<IAudioInService>() : SimpleIoc.Default.GetInstance<IAudioOutService>();

        /// <summary>
        /// Setup FFT.
        /// </summary>
        private void FftInitialize(object sender, RoutedEventArgs e)
        {
            // If FFT is enabled, setup render smoothers for each data point
            if (SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering))
            {
                BoundAudioService.AudioQueued += DataRecieved;

                _smoother1 = new Smoother(4, 6);
                _smoother2 = new Smoother(4, 12);
                _smoother3 = new Smoother(4, 14);
                _smoother4 = new Smoother(4, 14);
                _smoother5 = new Smoother(4, 15);
                _smoother6 = new Smoother(4, 16);
                _smoother7 = new Smoother(4, 16);
                _smoother8 = new Smoother(4, 15);
                _smoother9 = new Smoother(4, 14);
                _averageSmoother = new Smoother(1000, 100);

                _blurple = (Color)App.Current.Resources["BlurpleColor"];
                _transparentBlurple = (Color)App.Current.Resources["BlurpleColorTransparent"];
                initailized = true;
            }
        }

        /// <summary>
        /// Setup FFT async.
        /// </summary>
        private async void FftInitialize()
        {
            // Run on UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FftInitialize(null, null);
            });
        }

        /// <summary>
        /// Cleanup objects while disposed.
        /// </summary>
        private void FftDipose(object sender, RoutedEventArgs e)
        {
            // Clear FFT
            _smoother1 = null;
            _smoother2 = null;
            _smoother3 = null;
            _smoother4 = null;
            _smoother5 = null;
            _smoother6 = null;
            _smoother7 = null;
            _smoother8 = null;
            _smoother9 = null;
            _averageSmoother = null;
            initailized = false;

            // Unsubscribe from events
            BoundAudioService.AudioQueued -= DataRecieved;
            Loaded -= FftInitialize;
            Unloaded -= FftDipose;
        }

        /// <summary>
        /// Update Spec points.
        /// </summary>
        private void DataRecieved(object sender, float[] e)
        {
            // Determine FFT data
            float[] fftData = HelperMethods.GetFftChannelData(e, BoundAudioService.Samples);

            _specPoint0 = HelperMethods.Max(fftData, 0, 1);
            _specPoint1 = HelperMethods.Max(fftData, 2, 3);
            _specPoint2 = HelperMethods.Max(fftData, 3, 4);
            _specPoint3 = HelperMethods.Max(fftData, 4, 5);
            _specPoint4 = HelperMethods.Max(fftData, 5, 6);
            _specPoint5 = HelperMethods.Max(fftData, 7, 8);
            _specPoint6 = HelperMethods.Max(fftData, 9, 10);
            _specPoint7 = HelperMethods.Max(fftData, 10, 12);
            _specPoint8 = HelperMethods.Max(fftData, 14, 26);
            _specPointAverage = (_specPoint0 + _specPoint1 + _specPoint2 + _specPoint3 + _specPoint4 + _specPoint5 + _specPoint6 + _specPoint7 + _specPoint8) / 9;
        }

        /// <summary>
        /// Get left Curve point.
        /// </summary>
        /// <param name="input">Data point.</param>
        /// <returns>Point to render left bezier.</returns>
        private Vector2 GetC1(Vector2 input)
        {
            return new Vector2(input.X + _halfPoint, input.Y);
        }

        /// <summary>
        /// Get right Curve point.
        /// </summary>
        /// <param name="input">Data point.</param>
        /// <returns>point to render right bezier.</returns>
        private Vector2 GetC2(Vector2 input)
        {
            return new Vector2(input.X - _halfPoint, input.Y);
        }

        /// <summary>
        /// Adjust render value based on average.
        /// </summary>
        /// <param name="input">Data point.</param>
        /// <returns>Y Scale data point.</returns>
        private float Adjust(float input)
        {
            float multiplier = 1 + ((1 - average) * 4);
            if (multiplier < 1)
            {
                multiplier = 1;
            }

            return input * multiplier;
        }

        /// <summary>
        /// Draws each frame of the Canvas.
        /// </summary>
        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering))
            {
                if (!initailized)
                {
                    // If not initialized take one from to initialize
                    FftInitialize();
                }
                else
                {
                    using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                    {
                        // Start curve at 0
                        cpb.BeginFigure(0, _height);

                        // Initialize render points
                        Vector2 p0;
                        Vector2 p1;
                        Vector2 p2;
                        Vector2 p3;
                        Vector2 p4;
                        Vector2 p5;
                        Vector2 p6;
                        Vector2 p7;
                        Vector2 p8;

                        // Set render points
                        average = _averageSmoother.Smooth(_specPointAverage);
                        p0 = new Vector2(_point0, _height - (Adjust(_smoother1.Smooth(_specPoint0)) * _height));
                        p1 = new Vector2(_point1, _height - (Adjust(_smoother2.Smooth(_specPoint1)) * _height));
                        p2 = new Vector2(_point2, _height - (Adjust(_smoother3.Smooth(_specPoint2)) * _height));
                        p3 = new Vector2(_point3, _height - (Adjust(_smoother4.Smooth(_specPoint3)) * _height));
                        p4 = new Vector2(_point4, _height - (Adjust(_smoother5.Smooth(_specPoint4)) * _height));
                        p5 = new Vector2(_point5, _height - (Adjust(_smoother6.Smooth(_specPoint5)) * _height));
                        p6 = new Vector2(_point6, _height - (Adjust(_smoother7.Smooth(_specPoint6)) * _height));
                        p7 = new Vector2(_point7, _height - (Adjust(_smoother8.Smooth(_specPoint7)) * _height));
                        p8 = new Vector2(_point8, _height - (Adjust(_smoother9.Smooth(_specPoint8)) * _height));

                        // Render points
                        cpb.AddLine(p0);
                        cpb.AddCubicBezier(GetC1(p0), GetC2(p1), p1);
                        cpb.AddCubicBezier(GetC1(p1), GetC2(p2), p2);
                        cpb.AddCubicBezier(GetC1(p2), GetC2(p3), p3);
                        cpb.AddCubicBezier(GetC1(p3), GetC2(p4), p4);
                        cpb.AddCubicBezier(GetC1(p4), GetC2(p5), p5);
                        cpb.AddCubicBezier(GetC1(p5), GetC2(p6), p6);
                        cpb.AddCubicBezier(GetC1(p6), GetC2(p7), p7);
                        cpb.AddCubicBezier(GetC1(p7), GetC2(p8), p8);
                        cpb.AddLine(new Vector2(p8.X, _height));
                        cpb.EndFigure(CanvasFigureLoop.Closed);

                        // Render
                        CanvasLinearGradientBrush gradient = new CanvasLinearGradientBrush(sender, _transparentBlurple, _blurple)
                        {
                            EndPoint = new Vector2(0, _height + 48),
                            StartPoint = new Vector2(0, -12),
                        };
                        var path = CanvasGeometry.CreatePath(cpb);
                        args.DrawingSession.FillGeometry(path, gradient);
                    }
                }
            }
        }

        /// <summary>
        /// Adjust size of Control.
        /// </summary>
        private void CanvasAnimatedControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _height = (float)e.NewSize.Height - 1;
            float segment = (float)e.NewSize.Width / 8;
            _point1 = segment;
            _point2 = segment * 2;
            _point3 = segment * 3;
            _point4 = segment * 4;
            _point5 = segment * 5;
            _point6 = segment * 6;
            _point7 = segment * 7;
            _point8 = segment * 8;
            _halfPoint = segment / 2;
        }

        private class Smoother
        {
            private readonly float _multiplier = 0;
            private readonly float _smoothTime = 0;
            private float _previousVal = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="Smoother"/> class.
            /// </summary>
            /// <param name="SmoothTime">The smoothing window in *10ms.</param>
            /// /// <param name="multiplier">The opacity multiplier (5 by default).</param>
            public Smoother(int smoothTime, float multiplier = 5)
            {
                _smoothTime = smoothTime;
                _multiplier = multiplier;
            }

            /// <summary>
            /// If the difference with the previous sample isn't too big, This function uses a simple moving average formula to smooth the value out.
            /// </summary>
            public float Smooth(float input)
            {
                input = ((_previousVal * _smoothTime) + input) / (_smoothTime + 1);
                _previousVal = input;
                return input * _multiplier;
            }
        }
    }
}
