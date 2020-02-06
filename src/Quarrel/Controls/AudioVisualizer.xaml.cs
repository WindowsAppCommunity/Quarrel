using GalaSoft.MvvmLight.Ioc;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Quarrel.Helpers.AudioProcessing;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Voice.Audio;
using Quarrel.ViewModels.Services.Voice.Audio.In;
using Quarrel.ViewModels.Services.Voice.Audio.Out;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AcrylicBrush = Microsoft.UI.Xaml.Media.AcrylicBrush;

namespace Quarrel.Controls
{
    public sealed partial class AudioVisualizer : UserControl
    {
        public AudioVisualizer()
        {
            this.InitializeComponent();

            (Resources["AcrylicBlur"] as AcrylicBrush).TintLuminosityOpacity = 0;
            Loaded += fftInitialize;
            Unloaded += fftDipose;
        }

        #region Methods 

        #region Initialize and Deinstance

        /// <summary>
        /// Setup FFT
        /// </summary>
        private void fftInitialize(object sender, RoutedEventArgs e)
        {
            // If FFT is enabled, setup render smoothers for each data point
            if (SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering))
            {
                BoundAudioService.DataRecieved += DataRecieved;

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
                initailized = true;
            }
        }

        /// <summary>
        /// Setup FFT async
        /// </summary>
        private async void fftInitialize()
        {
            // Run on UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                fftInitialize(null, null);
            });
        }

        /// <summary>
        /// Cleanup objects while disposed
        /// </summary>
        private void fftDipose(object sender, RoutedEventArgs e)
        {
            // Clear FFT
            smoother1 = null;
            smoother2 = null;
            smoother3 = null;
            smoother4 = null;
            smoother5 = null;
            smoother6 = null;
            smoother7 = null;
            smoother8 = null;
            smoother9 = null;
            averageSmoother = null;
            initailized = false;

            // Unsubscribe from events
            BoundAudioService.DataRecieved -= DataRecieved;
            Loaded -= fftInitialize;
            Unloaded -= fftDipose;
        }

        #endregion

        /// <summary>
        /// Update Spec points 
        /// </summary>
        private void DataRecieved(object sender, float[] e)
        {
            // Determine FFT data
            float[] fftData = HelperMethods.GetFftChannelData(e, BoundAudioService.Samples);

            SpecPoint0 = HelperMethods.Max(fftData, 0, 1);
            SpecPoint1 = HelperMethods.Max(fftData, 2, 3);
            SpecPoint2 = HelperMethods.Max(fftData, 3, 4);
            SpecPoint3 = HelperMethods.Max(fftData, 4, 5);
            SpecPoint4 = HelperMethods.Max(fftData, 5, 6);
            SpecPoint5 = HelperMethods.Max(fftData, 7, 8);
            SpecPoint6 = HelperMethods.Max(fftData, 9, 10);
            SpecPoint7 = HelperMethods.Max(fftData, 10, 12);
            SpecPoint8 = HelperMethods.Max(fftData, 14, 26);
            SpecPointAverage = (SpecPoint0 + SpecPoint1 + SpecPoint2 + SpecPoint3 + SpecPoint4 + SpecPoint5 + SpecPoint6 + SpecPoint7 + SpecPoint8) / 9;
        }

        /// <summary>
        /// Get left Curve point
        /// </summary>
        /// <param name="input">Data point</param>
        /// <returns>Point to render left bezier</returns>
        Vector2 GetC1(Vector2 input)
        {
            return new Vector2(input.X + HalfPoint, input.Y);
        }

        /// <summary>
        /// Get right Curve point
        /// </summary>
        /// <param name="input">Data point</param>
        /// <returns>point to render right bezier</returns>
        Vector2 GetC2(Vector2 input)
        {
            return new Vector2(input.X - HalfPoint, input.Y);
        }

        /// <summary>
        /// Adjust render value based on average
        /// </summary>
        /// <param name="input">Data point</param>
        /// <returns>Y Scale data point</returns>
        float Adjust(float input)
        {
            float multiplier = 1 + ((1 - average) * 4);
            if (multiplier < 1) multiplier = 1;
            return input * multiplier;
        }

        /// <summary>
        /// Draws each frame of the Canvas
        /// </summary>
        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.GetValue<bool>(SettingKeys.ExpensiveRendering))
            {
                if (!initailized)
                {
                    // If not initialized take one from to initialize
                    fftInitialize();
                }
                else
                {
                    using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                    {
                        // Start curve at 0
                        cpb.BeginFigure(0, height);

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
                        average = averageSmoother.Smooth(SpecPointAverage);
                        p0 = new Vector2(Point0, height - Adjust(smoother1.Smooth(SpecPoint0)) * height);
                        p1 = new Vector2(Point1, height - Adjust(smoother2.Smooth(SpecPoint1)) * height);
                        p2 = new Vector2(Point2, height - Adjust(smoother3.Smooth(SpecPoint2)) * height);
                        p3 = new Vector2(Point3, height - Adjust(smoother4.Smooth(SpecPoint3)) * height);
                        p4 = new Vector2(Point4, height - Adjust(smoother5.Smooth(SpecPoint4)) * height);
                        p5 = new Vector2(Point5, height - Adjust(smoother6.Smooth(SpecPoint5)) * height);
                        p6 = new Vector2(Point6, height - Adjust(smoother7.Smooth(SpecPoint6)) * height);
                        p7 = new Vector2(Point7, height - Adjust(smoother8.Smooth(SpecPoint7)) * height);
                        p8 = new Vector2(Point8, height - Adjust(smoother9.Smooth(SpecPoint8)) * height);

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
                        cpb.AddLine(new Vector2(p8.X, height));
                        cpb.EndFigure(CanvasFigureLoop.Closed);

                        // Render
                        CanvasLinearGradientBrush gradient = new CanvasLinearGradientBrush(sender, TransparentBlurple, Blurple)
                        {
                            EndPoint = new Vector2(0, height + 48),
                            StartPoint = new Vector2(0, -12)
                        };
                        var path = CanvasGeometry.CreatePath(cpb);
                        //args.DrawingSession.DrawGeometry(path, Blurple, 1);
                        args.DrawingSession.FillGeometry(path, gradient);
                    }
                }
            }
        }

        /// <summary>
        /// Adjust size of Control
        /// </summary>
        private void CanvasAnimatedControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            height = (float)e.NewSize.Height - 1;
            float Segment = (float)e.NewSize.Width / 8;
            Point1 = Segment;
            Point2 = Segment * 2;
            Point3 = Segment * 3;
            Point4 = Segment * 4;
            Point5 = Segment * 5;
            Point6 = Segment * 6;
            Point7 = Segment * 7;
            Point8 = Segment * 8;
            HalfPoint = Segment / 2;
        }

        #endregion

        #region Properties

        #region Smoothers

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

        #endregion

        IAudioService BoundAudioService => Input ? (IAudioService)SimpleIoc.Default.GetInstance<IAudioInService>() : SimpleIoc.Default.GetInstance<IAudioOutService>();

        /// <summary>
        /// Indicates if incoming data should be rendered
        /// </summary>
        private bool initailized = false;

        /// <summary>
        /// Color to render polygon
        /// </summary>
        Color Blurple;

        /// <summary>
        /// Near the top it gradients to transparent. This needs to have the same RGB as <see cref="Blurple"/> but with 00 A
        /// </summary>
        Color TransparentBlurple;


        /// <summary>
        /// Height of object - 1 (will change if not 47)
        /// </summary>
        float height = 47;

        /// <summary>
        /// The middle offset of a data point
        /// </summary>
        float HalfPoint;

        #region Data Points

        float Point0 = 0;
        float Point1 = 0;
        float Point2 = 0;
        float Point3 = 0;
        float Point4 = 0;
        float Point5 = 0;
        float Point6 = 0;
        float Point7 = 0;
        float Point8 = 0;

        /// <summary>
        /// Average value of points
        /// </summary>
        float average = 0;

        #endregion

        #region Spec Points

        float SpecPoint0 = 0;
        float SpecPoint1 = 0;
        float SpecPoint2 = 0;
        float SpecPoint3 = 0;
        float SpecPoint4 = 0;
        float SpecPoint5 = 0;
        float SpecPoint6 = 0;
        float SpecPoint7 = 0;
        float SpecPoint8 = 0;

        /// <summary>
        /// Average value of Spec Points
        /// </summary>
        float SpecPointAverage = 0;

        #endregion

        /// <summary>
        /// Determines if the Visualizer is display input or output
        /// </summary>
        public bool Input { get; set; }
        #endregion

        #region Classes

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

            /// <summary>
            /// If the difference with the previous sample isn't too big, This function uses a simple moving average formula to smooth the value out
            /// </summary>
            public float Smooth(float input)
            {
                input = (((PreviousVal * SmoothTime) + input) / (SmoothTime + 1));

                PreviousVal = input;
                return input * Multiplier;
            }
        }


        #endregion
    }
}
