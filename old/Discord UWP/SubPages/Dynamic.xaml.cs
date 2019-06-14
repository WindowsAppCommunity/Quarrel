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
using Quarrel.LocalModels;
using Quarrel.Managers;
using Quarrel.Classes;
using MenuFlyoutItem = Windows.UI.Xaml.Controls.MenuFlyoutItem;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
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
        public bool CanBeFancy { get; set; }
        public bool ReadOnly { get; set; }
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
            if (!String.IsNullOrEmpty(data.PlaceHolderText) || !String.IsNullOrEmpty(data.StartText))
            {
                StringArg.Visibility = Visibility.Visible;
                StringArg.Text = data.StartText == null ? "" : data.StartText;
                StringArg.PlaceholderText = data.PlaceHolderText == null ? "" : data.PlaceHolderText;
                StringArg.IsReadOnly = data.ReadOnly;
            }

            if (data.CanBeFancy) MakeFancy.Visibility = Visibility.Visible;
            else MakeFancy.Visibility = Visibility.Collapsed;

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

        private void MakeFancy_OnClick(object sender, RoutedEventArgs e)
        {
            MenuFlyout flyout = new MenuFlyout();
            flyout.MenuFlyoutPresenterStyle = (Style)App.Current.Resources["MenuFlyoutPresenterStyle1"];
            string toconvert = StringArg.Text;
            if (string.IsNullOrWhiteSpace(StringArg.Text))
                toconvert = StringArg.PlaceholderText;
            FancyText fancy = new FancyText(FancyText.FindFancy(toconvert));
            foreach (var value in fancy.ConvertAll(toconvert))
            {
                var item = new MenuFlyoutItem()
                {
                    Text = value,
                    Style = (Style)Application.Current.Resources["MenuFlyoutItemStyle1"],
                };
                flyout.Items.Add(item);
                item.Click += Item_Click;
            }
                flyout.Items.Add(new MenuFlyoutItem()
                {
                    Text = toconvert,
                    Style = (Style)Application.Current.Resources["MenuFlyoutItemStyle1"],
        });
             flyout.ShowAt(sender as HyperlinkButton);
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            StringArg.Text = ((MenuFlyoutItem)sender).Text;
        }
    }
}
