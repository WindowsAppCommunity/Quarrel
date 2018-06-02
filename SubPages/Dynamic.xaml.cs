using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class SubPageData
    {
        public string Message { get; set; }
        public string SubMessage { get; set; }
        public string ConfirmMessage { get; set; }
        public string StartText { get; set; }
        public string PlaceHolderText { get; set; }
        public bool ConfirmRed { get; set; }
        public object args { get; set; }
        public Func<object, object> function { get; set; }
    }

    public sealed partial class DynamicSubPage : Page
    {
        public DynamicSubPage()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        SubPageData data;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            data = (e.Parameter as SubPageData);

            Message.Text = data.Message;
            if (data.PlaceHolderText != null)
            {
                StringArg.Visibility = Visibility.Visible;
                StringArg.Text = data.StartText;
                StringArg.PlaceholderText = data.PlaceHolderText;
            }
            SubMessage.Text = data.SubMessage;

            if (data.ConfirmMessage == "") { ConfirmButton.Visibility = Visibility.Collapsed; }
            else { ConfirmButton.Content = data.ConfirmMessage; }
            if (data.ConfirmRed) { ConfirmButton.Background = (Brush)App.Current.Resources["dnd"]; }

        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (data.PlaceHolderText != null && data.args is List<object>)
            {
                (data.args as List<object>).Add(StringArg.Text);
            }
            data.function(data.args);
            CloseButton_Click(null, null);
        }
    }
}
