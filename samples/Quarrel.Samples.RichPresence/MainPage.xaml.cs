// Quarrel © 2022

using System;
using Quarrel.RichPresence;
using Quarrel.RichPresence.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Samples.RichPresence
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RichPresenceConnection _connection;

        public MainPage()
        {
            this.InitializeComponent();
            _connection = new RichPresenceConnection();
            _connection.Closed += async (o, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    UnconnectedGrid.Visibility = Visibility.Visible;
                    ConnectedGrid.Visibility = Visibility.Collapsed;
                });
            };
        }

        private async void Connect(object sender, RoutedEventArgs e)
        {
            bool success = await _connection.ConnectAsync(AppVersionType.Dev);

            if (success)
            {
                UnconnectedGrid.Visibility = Visibility.Collapsed;
                ConnectedGrid.Visibility = Visibility.Visible;
            }
        }

        private async void SetActivity(object sender, RoutedEventArgs e)
        {
            Activity activity = new Activity(ActivityName.Text);
            await _connection.SetActivity(activity);
        }
    }
}
