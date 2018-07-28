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
using Discord_UWP.Classes;
using Discord_UWP.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiscordStatus : Page
    {
        public class SimpleComponent
        {
            public string Description { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public SolidColorBrush Color { get; set; }
        }
        public DiscordStatus()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
            _chartRenderer = new ChartRenderer();
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!OfflineMode)
            {
                scale.CenterY = this.ActualHeight / 2;
                scale.CenterX = this.ActualWidth / 2;
                NavAway.Begin();
                App.SubpageClosed();
            }
            else
            {
                Frame.Navigate(typeof(Offline), status);
            }
        }

        private StatusPageClasses.Index status = null;
        public SolidColorBrush ColorFromStatus(string status)
        {
            if (status == "operational" || status == "none")
            {
                return (SolidColorBrush)Application.Current.Resources["online"];
            }
            else if (status == "partial_outage")
            {
                return (SolidColorBrush)Application.Current.Resources["idle"];
            }
            else
            {
                return (SolidColorBrush)Application.Current.Resources["dnd"];
            }
            
        }

        private bool OfflineMode = false;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                OfflineMode = true;
                cache.Fill = (SolidColorBrush) Application.Current.Resources["AcrylicMessageBackground"];
            }
            grid.Opacity = 0;
            grid.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = true;
            base.OnNavigatedTo(e);

            LoadingRing.Visibility = Visibility.Visible;
            status = await StatusPage.GetStatus();
           
            if (status == null)
            {
                FailedToLoad.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                FailedToLoad.Visibility = Visibility.Collapsed;
            }

            if (status.Status != null)
            {
                var statusColor = ColorFromStatus(status.Status.Indicator);
                statusContainer.Background = statusColor;
                border.BorderBrush = statusColor;
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
            grid.Visibility = Visibility.Visible;
            scale2.CenterY = grid.ActualHeight / 2;
            scale2.CenterX = grid.ActualWidth / 2;
            LoadIn.Begin();
            ShowMetrics("day");
        }

        private bool ChangedMetricsDisplay = false;
        private async void ShowMetrics(string duration)
        {
            if (ChangedMetricsDisplay)
            {
                HideChart.Begin();
            }

            ChangedMetricsDisplay = true;
            var metrics = await StatusPage.GetMetrics(duration);
            if (metrics != null && metrics.Metrics != null && metrics.Metrics.Length > 0)
            {
                var metric = metrics.Metrics[0];
                int pos = 0;
                _data.Clear();
                _max = 0;
                _min = 0;
                for (var i = 0; i < metric.Data.Length; i++)
                {
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
                if(HideChart.GetCurrentState() != ClockState.Stopped)
                {
                    HideChart.Stop();
                }
                ShowChart.Begin();
            }
        }
        private readonly List<double> _data = new List<double>();
        private const float DataStrokeThickness = 1;
        private Color blurple = (Color) Application.Current.Resources["BlurpleColor"];
        private readonly ChartRenderer _chartRenderer;
        private double _min = 0;
        private double _max = 0;
        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            _chartRenderer.RenderData(chartCanvas, args, blurple, DataStrokeThickness, _data, false, _max);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartClip.Rect = new Rect(-chartCanvas.ActualWidth, 0, chartCanvas.ActualWidth, chartCanvas.ActualHeight);
            ShowChartDa.To = chartCanvas.ActualWidth;
            HideChartDa.From = chartCanvas.ActualWidth;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMetrics("day");
            dayDuration.IsEnabled = false;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = true;
            //Day
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            ShowMetrics("week");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = false;
            monthDuration.IsEnabled = true;
            //Week
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            ShowMetrics("month");
            dayDuration.IsEnabled = true;
            weekDuration.IsEnabled = true;
            monthDuration.IsEnabled = false;
            //Month
        }
    }
}
