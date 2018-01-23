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

namespace Discord_UWP.Controls
{
    public sealed partial class TypingIndicator : UserControl
    {
        public bool IsWhite
        {
            set
            {
                if(value)
                {
                    ellipse.Fill = new SolidColorBrush(Colors.White);
                    ellipse1.Fill = new SolidColorBrush(Colors.White);
                    ellipse2.Fill = new SolidColorBrush(Colors.White);

                }
            }
        }
        public TypingIndicator()
        {
            this.InitializeComponent();
            Typing.Begin();
        }
    }
}
