// Copyright (c) Quarrel. All rights reserved.

using Quarrel.SubPages.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to show Quarrel and Quarrel's libraries licensing information.
    /// </summary>
    public sealed partial class LicensesPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensesPage"/> class.
        /// </summary>
        public LicensesPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 512;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        private void ToggleMIT(object sender, RoutedEventArgs e)
        {
            if (mitlicense.Visibility == Visibility.Visible)
            {
                mitlicense.Visibility = Visibility.Collapsed;
            }
            else
            {
                mitlicense.Visibility = Visibility.Visible;
            }
        }

        private void ToggleMSPL(object sender, RoutedEventArgs e)
        {
            if (mspllicense.Visibility == Visibility.Visible)
            {
                mspllicense.Visibility = Visibility.Collapsed;
            }
            else
            {
                mspllicense.Visibility = Visibility.Visible;
            }
        }

        private void ToggleConcentus(object sender, RoutedEventArgs e)
        {
            if (concentuslicense.Visibility == Visibility.Visible)
            {
                concentuslicense.Visibility = Visibility.Collapsed;
            }
            else
            {
                concentuslicense.Visibility = Visibility.Visible;
            }
        }

        private void ToggleAPACHEL(object sender, RoutedEventArgs e)
        {
            if (apachelicense.Visibility == Visibility.Visible)
            {
                apachelicense.Visibility = Visibility.Collapsed;
            }
            else
            {
                apachelicense.Visibility = Visibility.Visible;
            }
        }
    }
}
