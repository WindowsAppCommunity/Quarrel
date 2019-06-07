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

namespace Quarrel.Controls
{
    public sealed partial class MiniWarning : UserControl
    {

        /// <summary>
        /// Basic warning message
        /// </summary>
        string Details;

        /// <summary>
        /// Advanced warning message
        /// </summary>
        string Explanation;

        /// <summary>
        /// Initalizes
        /// </summary>
        /// <param name="details"></param>
        /// <param name="explanation"></param>
        public MiniWarning(string details, string explanation)
        {
            Details = details;
            Explanation = explanation;
            InitializeComponent();
        }

        /// <summary>
        /// Show explanation
        /// </summary>
        private void details_Click(object sender, RoutedEventArgs e)
        {
           ((HyperlinkButton)sender).ContextFlyout.ShowAt((HyperlinkButton)sender);
        }

        /// <summary>
        /// Load UI
        /// </summary>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            details.Content = Details;
            explanation.Text = Explanation;
        }
    }
}
