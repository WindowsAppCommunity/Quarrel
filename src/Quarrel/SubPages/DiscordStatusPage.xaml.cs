using DiscordStatusAPI.Models;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.Helpers;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.SubPages;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Quarrel.SubPages
{
    public sealed partial class DiscordStatusPage : UserControl, IConstrainedSubPage
    {
        public DiscordStatusPage()
        {
            this.InitializeComponent();
            DataContext = new DiscordStatusPageViewModel();

            ViewModel.ChartDataLoaded += (m, args) =>
            {
                chartCanvas.Invalidate();
                if (HideChart.GetCurrentState() != ClockState.Stopped)
                {
                    HideChart.Stop();
                }
                ShowChart.Begin();
            };

            // Change everythings color by status
            ViewModel.StatusLoaded += (m, args) =>
            {
                var statusBrush = new SolidColorBrush(StatusColor);
                StatusContainer.Background = statusBrush;
                detailsButton.Foreground = statusBrush;
                dayDuration.Foreground = statusBrush;
                weekDuration.Foreground = statusBrush;
                monthDuration.Foreground = statusBrush;
                IncidentsScroller.Background = new SolidColorBrush(statusBrush.Color) { Opacity = 0.25 };
            };
        }

        public DiscordStatusPageViewModel ViewModel => DataContext as DiscordStatusPageViewModel;

        #region Methods

        public Color ColorFromStatus(string status)
        {
            if (status == "operational" || status == "none")
            {
                return (Color)App.Current.Resources["onlineColor"];
            }
            else if (status == "partial_outage" | status == "minor")
            {
                return (Color)App.Current.Resources["idleColor"];
            }
            else
            {
                return (Color)App.Current.Resources["dndColor"];
            }
        }

        #region Chart Render

        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ViewModel.Loaded)
                _chartRenderer.RenderData(chartCanvas, args, StatusColor, DataStrokeThickness, ViewModel.Data, false, ViewModel.Max);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartClip.Rect = new Rect(-chartCanvas.ActualWidth, 0, chartCanvas.ActualWidth, chartCanvas.ActualHeight);
            chartClipTransform.X = chartCanvas.ActualWidth;
            ShowChartDa.To = chartCanvas.ActualWidth;
            HideChartDa.From = chartCanvas.ActualWidth;
        }
        
        #endregion

        #region Change Duration

        /// <summary>
        /// Selects and loads day graph
        /// </summary>
        private void ShowDayMetrics(object sender, RoutedEventArgs e)
        {
            HideChart.Begin();
            ViewModel.ShowMetrics("day");
            dayDuration.IsEnabled = false;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = true;
        }

        /// <summary>
        /// Selects and loads week graph
        /// </summary>
        private void ShowWeekMetrics(object sender, RoutedEventArgs e)
        {
            HideChart.Begin();
            ViewModel.ShowMetrics("week");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = false;
            monthDuration.IsEnabled = true;
        }

        /// <summary>
        /// Selects and loads month graph
        /// </summary>
        private void ShowMonthMetrics(object sender, RoutedEventArgs e)
        {
            HideChart.Begin();
            ViewModel.ShowMetrics("month");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = false;
        }

        #endregion

        #region Indicator
        private void chartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            CursorPosition = Convert.ToSingle(e.GetCurrentPoint(chartCanvas).Position.X);
            chartIndicator.Invalidate();

        }

        private void ChartIndicator_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (_chartRenderer.stepsize != 0)
            {
                var location = Convert.ToInt32(Math.Round(CursorPosition / _chartRenderer.stepsize));
                if (ViewModel.DataValues.ContainsKey(location))
                {
                    var item = ViewModel.DataValues[location];
                    CanvasTextFormat format = new CanvasTextFormat { FontSize = 12.0f, WordWrapping = CanvasWordWrapping.NoWrap };
                    CanvasTextLayout textLayout = new CanvasTextLayout(args.DrawingSession, item.Value + "ms", format, 0.0f, 0.0f);

                    CanvasTextFormat format2 = new CanvasTextFormat { FontSize = 12.0f, WordWrapping = CanvasWordWrapping.NoWrap };

                    DateTimeOffset date = DateTimeOffset.FromUnixTimeSeconds(item.Timestamp);
                    string durationText;
                    if (!dayDuration.IsEnabled)
                    {
                        durationText = date.ToString("t");
                    }
                    else if (!weekDuration.IsEnabled)
                    {
                        durationText = date.DayOfWeek.ToString() + " " + date.ToString("t");
                    }
                    else
                    {
                        durationText = date.ToString("g");
                    }
                    CanvasTextLayout textLayout2 = new CanvasTextLayout(args.DrawingSession, durationText, format, 0.0f, 0.0f);

                    if (CursorPosition + textLayout2.DrawBounds.Width + 6 > chartIndicator.ActualWidth || CursorPosition + textLayout.DrawBounds.Width + 6 > chartIndicator.ActualWidth)
                    {
                        args.DrawingSession.DrawTextLayout(textLayout, new Vector2(Convert.ToSingle((CursorPosition - textLayout.DrawBounds.Width - 12)), 0), Color.FromArgb(255, 255, 255, 255));
                        args.DrawingSession.DrawTextLayout(textLayout2, new Vector2(Convert.ToSingle((CursorPosition - textLayout2.DrawBounds.Width - 12)), 14), Color.FromArgb(120, 255, 255, 255));
                    }
                    else
                    {
                        args.DrawingSession.DrawTextLayout(textLayout, new Vector2(CursorPosition + 4, 0), Color.FromArgb(255, 255, 255, 255));
                        args.DrawingSession.DrawTextLayout(textLayout2, new Vector2(CursorPosition + 4, 14), Color.FromArgb(120, 255, 255, 255));
                    }
                }
                args.DrawingSession.DrawLine(new Vector2(CursorPosition, 0), new Vector2(CursorPosition, (float)chartIndicator.ActualHeight), Colors.White);

            }
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            chartIndicator.Fade(1, 300).Start();
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            chartIndicator.Fade(0, 300).Start();
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Width of line on graph
        /// </summary>
        private const float DataStrokeThickness = 1;

        /// <summary>
        /// The position of the mouse on the chart, in Pixels
        /// </summary>
        private float CursorPosition;

        /// <summary>
        /// Accent Color based on Status
        /// </summary>
        private Color StatusColor => ViewModel.Status != null ?
            ColorFromStatus(ViewModel.Status.Status.Indicator) :
            (Color)App.Current.Resources["SystemAccentColor"];

        /// <summary>
        /// Rendering help for the graph
        /// </summary>
        private readonly ChartRenderer _chartRenderer = new ChartRenderer();

        #endregion

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;
        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
