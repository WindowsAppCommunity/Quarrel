// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.Helpers;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.SubPages;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Constants = Quarrel.ViewModels.Helpers.Constants;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to display the status of Discord's servers.
    /// </summary>
    public sealed partial class DiscordStatusPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Width of line on graph.
        /// </summary>
        private const float DataStrokeThickness = 1;

        private readonly ChartRenderer _chartRenderer = new ChartRenderer();

        private IAnalyticsService _analyticsService = null;

        /// <summary>
        /// The position of the mouse on the chart, in Pixels.
        /// </summary>
        private float _cursorPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordStatusPage"/> class.
        /// </summary>
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

            // Change everything's color by status
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

            AnalyticsService.Log(Constants.Analytics.Events.OpenDiscordStatus);
        }

        /// <summary>
        /// Gets the Discord API status.
        /// </summary>
        public DiscordStatusPageViewModel ViewModel => DataContext as DiscordStatusPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 512;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        /// <summary>
        /// Gets Accent Color based on Status.
        /// </summary>
        private Color StatusColor => ViewModel.Status != null ?
            ColorFromStatus(ViewModel.Status.Status.Indicator) :
            (Color)App.Current.Resources["SystemAccentColor"];

        /// <summary>
        /// Takes an API stauts and returns the corresponding color.
        /// </summary>
        /// <param name="status">API Status.</param>
        /// <returns><paramref name="status"/>'s color.</returns>
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

        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ViewModel.Loaded)
            {
                _chartRenderer.RenderData(chartCanvas, args, StatusColor, DataStrokeThickness, ViewModel.Data, false, ViewModel.Max);
            }
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartClip.Rect = new Rect(-chartCanvas.ActualWidth, 0, chartCanvas.ActualWidth, chartCanvas.ActualHeight);
            chartClipTransform.X = chartCanvas.ActualWidth;
            ShowChartDa.To = chartCanvas.ActualWidth;
            HideChartDa.From = chartCanvas.ActualWidth;
        }

        /// <summary>
        /// Selects and loads day graph.
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
        /// Selects and loads week graph.
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
        /// Selects and loads month graph.
        /// </summary>
        private void ShowMonthMetrics(object sender, RoutedEventArgs e)
        {
            HideChart.Begin();
            ViewModel.ShowMetrics("month");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = false;
        }

        private void ChartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _cursorPosition = Convert.ToSingle(e.GetCurrentPoint(chartCanvas).Position.X);
            chartIndicator.Invalidate();
        }

        private void ChartIndicator_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (_chartRenderer.StepSize != 0)
            {
                var location = Convert.ToInt32(Math.Round(_cursorPosition / _chartRenderer.StepSize));
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

                    if (_cursorPosition + textLayout2.DrawBounds.Width + 6 > chartIndicator.ActualWidth || _cursorPosition + textLayout.DrawBounds.Width + 6 > chartIndicator.ActualWidth)
                    {
                        args.DrawingSession.DrawTextLayout(textLayout, new Vector2(Convert.ToSingle(_cursorPosition - textLayout.DrawBounds.Width - 12), 0), Color.FromArgb(255, 255, 255, 255));
                        args.DrawingSession.DrawTextLayout(textLayout2, new Vector2(Convert.ToSingle(_cursorPosition - textLayout2.DrawBounds.Width - 12), 14), Color.FromArgb(120, 255, 255, 255));
                    }
                    else
                    {
                        args.DrawingSession.DrawTextLayout(textLayout, new Vector2(_cursorPosition + 4, 0), Color.FromArgb(255, 255, 255, 255));
                        args.DrawingSession.DrawTextLayout(textLayout2, new Vector2(_cursorPosition + 4, 14), Color.FromArgb(120, 255, 255, 255));
                    }
                }

                args.DrawingSession.DrawLine(new Vector2(_cursorPosition, 0), new Vector2(_cursorPosition, (float)chartIndicator.ActualHeight), Colors.White);
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
    }
}
