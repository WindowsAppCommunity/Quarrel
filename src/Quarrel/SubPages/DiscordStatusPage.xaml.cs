using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.Helpers;
using DiscordStatusAPI.Models;
using DiscordStatusAPI.API.Status;
using DiscordStatusAPI.API;
using Quarrel.SubPages.Interfaces;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class DiscordStatusPage : UserControl, IConstrainedSubPage
    {
        #region Classes

        public class ComplexComponent
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public string Time { get; set; }
            public List<SimpleComponent> Items { get; set; }
            public SolidColorBrush Color { get; set; }
        }
        public class SimpleComponent
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public SolidColorBrush Color { get; set; }
        }

        #endregion

        #region Constructors
        public DiscordStatusPage()
        {
            this.InitializeComponent();
            Setup();
        }
        #endregion

        #region Methods
        private async void Setup()
        {
            RestFactory factory = new RestFactory();
            StatusService = factory.GetStatusService();

            status = await StatusService.GetStatus();

            if (status == null)
            {
                FailedToLoad.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                FailedToLoad.Visibility = Visibility.Collapsed;
            }
            if (status.Incidents != null)
            {
                foreach (var incident in status.Incidents)
                {
                    if (incident.Status != "resolved")
                    {
                        List<SimpleComponent> updates = new List<SimpleComponent>();
                        for (int i = 0; i < incident.IncidentUpdates.Length; i++)
                        {
                            updates.Add(new SimpleComponent()
                            {
                                Status = incident.IncidentUpdates[i].Status,
                                Description = incident.IncidentUpdates[i].Body,
                                Name = incident.IncidentUpdates[i].UpdatedAt.ToString("t")
                            });
                        }
                        ComplexComponent sc = new ComplexComponent()
                        {
                            Name = incident.Name,
                            Status = incident.Status,
                            Color = ColorFromStatus(incident.Status),
                            Items = updates
                        };

                        if (!string.IsNullOrWhiteSpace(sc.Name))
                            IncidentsPanel.Items.Add(sc);
                    }
                }
            }

            if (IncidentsPanel.Items.Count > 0)
            {
                IncidentsPanel.Visibility = Visibility.Visible;
                IncidentsScroller.Visibility = Visibility.Visible;
            }
            else
            {
                IncidentsPanel.Visibility = Visibility.Collapsed;
                IncidentsScroller.Visibility = Visibility.Collapsed;
            }

            if (status.Status != null)
            {
                var statusBrush = ColorFromStatus(status.Status.Indicator);
                statusColor = statusBrush.Color;
                statusContainer.Background = statusBrush;
                dayDuration.Foreground = statusBrush;
                weekDuration.Foreground = statusBrush;
                monthDuration.Foreground = statusBrush;
                IncidentsScroller.Background = new SolidColorBrush(statusBrush.Color) { Opacity = 0.25 };
                statusDescription.Text = status.Status.Description;
                statusContainer.Visibility = Visibility.Visible;
            }
            if (status.Components != null)
            {
                foreach (var component in status.Components)
                {
                    SimpleComponent sc = new SimpleComponent()
                    {
                        Name = component.Name,
                        Status = component.Status.Replace("_", " "),
                        Color = ColorFromStatus(component.Status),
                        Description = component.Description
                    };
                    ComponentsPanel.Items.Add(sc);

                }
            }
            scale2.CenterY = grid.ActualHeight / 2;
            scale2.CenterX = grid.ActualWidth / 2;
            LoadIn.Begin();
            ShowMetrics("day");
        }

        public SolidColorBrush ColorFromStatus(string status)
        {
            if (status == "operational" || status == "none")
            {
                return (SolidColorBrush)Application.Current.Resources["online"];
            }
            else if (status == "partial_outage" | status == "minor")
            {
                return (SolidColorBrush)Application.Current.Resources["idle"];
            }
            else
            {
                return (SolidColorBrush)Application.Current.Resources["dnd"];
            }

        }

        private async void ShowMetrics(string duration)
        {
            if (ChangedMetricsDisplay)
            {
                HideChart.Begin();
            }

            ChangedMetricsDisplay = true;
            var metrics = await StatusService.GetMetrics(duration);
            if (metrics != null && metrics.Metrics != null && metrics.Metrics.Length > 0)
            {

                var metric = metrics.Metrics[0];
                _data.Clear();
                datavalues.Clear();
                _max = 0;
                _min = 0;
                for (var i = 0; i < metric.Data.Length; i++)
                {
                    datavalues.Add(i, metric.Data[i]);
                    _data.Add(metric.Data[i].Value);
                    if (metric.Data[i].Value > _max)
                    {
                        _max = metric.Data[i].Value;
                    }
                    if (metric.Data[i].Value < _min || _min == 0)
                    {
                        _min = metric.Data[i].Value;
                    }

                }
                chartCanvas.Invalidate();
                if (HideChart.GetCurrentState() != ClockState.Stopped)
                {
                    HideChart.Stop();
                }
                ShowChart.Begin();
            }
        }

        #region Chart Render
        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            _chartRenderer.RenderData(chartCanvas, args, statusColor, DataStrokeThickness, _data, false, _max);
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
        private void ShowDayMetrics(object sender, RoutedEventArgs e)
        {
            ShowMetrics("day");
            dayDuration.IsEnabled = false;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = true;
            //Day
        }

        private void ShowWeekMetrics(object sender, RoutedEventArgs e)
        {
            ShowMetrics("week");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = false;
            monthDuration.IsEnabled = true;
            //Week
        }

        private void ShowMonthMetrics(object sender, RoutedEventArgs e)
        {
            ShowMetrics("month");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = false;
            //Month
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
                if (datavalues.ContainsKey(location))
                {
                    var item = datavalues[location];
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

        // Outage index information
        private Index status = null;

        // Indicates if the chart has been changed
        private bool ChangedMetricsDisplay = false;
        
        // Ping times to graph
        Dictionary<int, Datum> datavalues = new Dictionary<int, Datum>();
        private readonly List<double> _data = new List<double>();

        // Width of line on graph
        private const float DataStrokeThickness = 1;

        // Lowest value on chart
        private double _min = 0;
        
        // Highest value on chart
        private double _max = 0;
        
        // The position of the mouse on the chart, in Pixels
        private float CursorPosition;

        // Accent Color based on Status
        private Color statusColor = (Color)Application.Current.Resources["BlurpleColor"];
        
        // Access to Discord Status API
        private IStatusService StatusService;

        // Rendering help for the graph
        private readonly ChartRenderer _chartRenderer = new ChartRenderer();

        #endregion

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;
        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
