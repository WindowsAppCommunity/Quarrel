using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class TypingIndicator : UserControl
    {
        /// <summary>
        /// If true, set to white
        /// </summary>
        public bool IsWhite
        {
            set
            {
                if(value)
                {
                    ellipse.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    ellipse1.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    ellipse2.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                }
            }
        }

        /// <summary>
        /// Render and begin indicator animation
        /// </summary>
        public TypingIndicator()
        {
            this.InitializeComponent();
            Typing.Begin();
        }

        /// <summary>
        /// Dispose of Control
        /// </summary>
        public void Dispose()
        {
            // Nothing to dipose
        }
    }
}
