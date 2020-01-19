using Quarrel.SubPages.Interfaces;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class LicensesPage : UserControl, IConstrainedSubPage
    {
        public LicensesPage()
        {
            this.InitializeComponent();
        }

        private void ToggleMIT(object sender, RoutedEventArgs e)
        {
            if (mitlicense.Visibility == Visibility.Visible)
                mitlicense.Visibility = Visibility.Collapsed;
            else
                mitlicense.Visibility = Visibility.Visible;
        }

        private void ToggleMSPL(object sender, RoutedEventArgs e)
        {
            if (mspllicense.Visibility == Visibility.Visible)
                mspllicense.Visibility = Visibility.Collapsed;
            else
                mspllicense.Visibility = Visibility.Visible;
        }

        private void ToggleConcentus(object sender, RoutedEventArgs e)
        {
            if (concentuslicense.Visibility == Visibility.Visible)
                concentuslicense.Visibility = Visibility.Collapsed;
            else
                concentuslicense.Visibility = Visibility.Visible;
        }

        private void ToggleAPACHEL(object sender, RoutedEventArgs e)
        {
            if (apachelicense.Visibility == Visibility.Visible)
                apachelicense.Visibility = Visibility.Collapsed;
            else
                apachelicense.Visibility = Visibility.Visible;
        }

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 512;
        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
