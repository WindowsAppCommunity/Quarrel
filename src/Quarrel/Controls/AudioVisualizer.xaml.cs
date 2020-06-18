// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Voice;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Quarrel.Helpers.AudioProcessing;
using Quarrel.ViewModels.Services.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Quarrel.ViewModels.Services.Voice;
using AcrylicBrush = Microsoft.UI.Xaml.Media.AcrylicBrush;

namespace Quarrel.Controls
{
    /// <summary>
    /// Control that displays a frourier transform of a PCM audio stream.
    /// </summary>
    public sealed partial class AudioVisualizer : UserControl
    {
        /// <summary>
        /// The number of approximate frequencies to render in the visualizer.
        /// </summary>
        private const int _count = 9;

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

        private float[] _points = new float[_count];

        /// <summary>
        /// Average value of points.
        /// </summary>
        private float average = 0;

        private float[] _specPoints = new float[_count];

        /// <summary>
        /// Average value of Spec Points.
        /// </summary>
        private float _specPointAverage = 0;

        private Smoother[] _smoothers = new Smoother[_count];
        private Smoother _averageSmoother;

        private CanvasLinearGradientBrush _gradient;

        private bool _input;

        private bool _expensiveRendering;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioVisualizer"/> class.
        /// </summary>
        public AudioVisualizer()
        {
            this.InitializeComponent();

            (Resources["AcrylicBlur"] as AcrylicBrush).TintLuminosityOpacity = 0;
            Loaded += FftInitialize;
            Unloaded += FftDipose;
            _expensiveRendering = SettingsService.Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering);

            // Todo: handle settings changes
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Visualizer is displaying input or output.
        /// </summary>
        public bool Input
        {
            get => _input;
            set
            {
                _input = value;
                if (value)
                {
                    VoiceService.AudioInData += DataRecieved;
                    VoiceService.AudioOutData -= DataRecieved;
                }
                else
                {
                    VoiceService.AudioInData -= DataRecieved;
                    VoiceService.AudioOutData += DataRecieved;
                }
            }
        }

        private ISettingsService SettingsService { get; } = SimpleIoc.Default.GetInstance<ISettingsService>();
        private IVoiceService VoiceService { get; } = SimpleIoc.Default.GetInstance<IVoiceService>();


        /// <summary>
        /// Setup FFT.
        /// </summary>
        private void FftInitialize(object sender, RoutedEventArgs e)
        {
            // If FFT is enabled, setup render smoothers for each data point
            if (_expensiveRendering)
            {
                _smoothers[0] = new Smoother(4, 6);
                _smoothers[1] = new Smoother(4, 12);
                _smoothers[2] = new Smoother(4, 14);
                _smoothers[3] = new Smoother(4, 14);
                _smoothers[4] = new Smoother(4, 15);
                _smoothers[5] = new Smoother(4, 16);
                _smoothers[6] = new Smoother(4, 16);
                _smoothers[7] = new Smoother(4, 15);
                _smoothers[8] = new Smoother(4, 14);
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
            for (int i = 0; i < _count; i++)
            {
                _smoothers[i] = null;
            }

            _averageSmoother = null;
            initailized = false;

            // Unsubscribe from events
            VoiceService.AudioInData -= DataRecieved;
            VoiceService.AudioOutData -= DataRecieved;
            Loaded -= FftInitialize;
            Unloaded -= FftDipose;
        }

        /// <summary>
        /// Update Spec points.
        /// </summary>
        private void DataRecieved(object sender, IList<float> e)
        {
            float[] data = new float[512];
            for (int i = 0; i < e.Count; i++)
            {
                data[i] = e[i] / 32768;
            }

            // Determine FFT data
            float[] fftData = HelperMethods.GetFftChannelData(data);

            _specPoints[0] = HelperMethods.Max(fftData, 0, 1);
            _specPoints[1] = HelperMethods.Max(fftData, 2, 3);
            _specPoints[2] = HelperMethods.Max(fftData, 3, 4);
            _specPoints[3] = HelperMethods.Max(fftData, 4, 5);
            _specPoints[4] = HelperMethods.Max(fftData, 5, 6);
            _specPoints[5] = HelperMethods.Max(fftData, 7, 8);
            _specPoints[6] = HelperMethods.Max(fftData, 9, 10);
            _specPoints[7] = HelperMethods.Max(fftData, 10, 12);
            _specPoints[8] = HelperMethods.Max(fftData, 14, 26);
            _specPointAverage = _specPoints.Average();
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
        private void CanvasAnimatedControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (_expensiveRendering)
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
                        Vector2[] p = new Vector2[_count];

                        // Set render points
                        average = _averageSmoother.Smooth(_specPointAverage);
                        for (int i = 0; i < _count; i++)
                        {
                            p[i] = new Vector2(_points[i], _height - (Adjust(_smoothers[i].Smooth(_specPoints[i])) * _height));
                        }

                        // Render points
                        cpb.AddLine(p[0]);
                        for (int i = 0; i < _count - 1; i++)
                        {
                            cpb.AddCubicBezier(GetC1(p[i]), GetC2(p[i + 1]), p[i + 1]);
                        }

                        cpb.AddLine(new Vector2(p[_count - 1].X, _height));
                        cpb.EndFigure(CanvasFigureLoop.Closed);

                        var path = CanvasGeometry.CreatePath(cpb);
                        args.DrawingSession.FillGeometry(path, _gradient);
                    }
                }
            }
        }

        private void CanvasAnimatedControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            _gradient = new CanvasLinearGradientBrush(sender, _transparentBlurple, _blurple)
            {
                EndPoint = new Vector2(0, _height + 48),
                StartPoint = new Vector2(0, -12),
            };
        }

        /// <summary>
        /// Adjust size of Control.
        /// </summary>
        private void CanvasAnimatedControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _height = (float)e.NewSize.Height - 1;
            float segment = (float)e.NewSize.Width / 8;
            for (int i = 1; i < _count; i++)
            {
                _points[i] = segment * i;
            }

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
            /// <param name="smoothTime">The smoothing window in *10ms.</param>
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
