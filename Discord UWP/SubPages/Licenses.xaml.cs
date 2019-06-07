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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Licenses : Page
    {
        public Licenses()
        {
            this.InitializeComponent();

            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;

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
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mitlicense.Visibility == Visibility.Visible)
                mitlicense.Visibility = Visibility.Collapsed;
            else
                mitlicense.Visibility = Visibility.Visible;
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (mspllicense.Visibility == Visibility.Visible)
                mspllicense.Visibility = Visibility.Collapsed;
            else
                mspllicense.Visibility = Visibility.Visible;
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (concentuslicense.Visibility == Visibility.Visible)
                concentuslicense.Visibility = Visibility.Collapsed;
            else
                concentuslicense.Visibility = Visibility.Visible;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (apachelicense.Visibility == Visibility.Visible)
                apachelicense.Visibility = Visibility.Collapsed;
            else
                apachelicense.Visibility = Visibility.Visible;
        }
    }
}
