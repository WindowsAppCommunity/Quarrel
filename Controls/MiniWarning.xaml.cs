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

namespace Discord_UWP.Controls
{
    public sealed partial class MiniWarning : UserControl
    {

        string Details;
        string Explanation;
        public MiniWarning(string details, string explanation)
        {
            Details = details;
            Explanation = explanation;
            InitializeComponent();
        }

        private void details_Click(object sender, RoutedEventArgs e)
        {
           ((HyperlinkButton)sender).ContextFlyout.ShowAt((HyperlinkButton)sender);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            details.Content = Details;
            explanation.Text = Explanation;
        }
    }
}
