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

namespace Quarrel.Controls
{
    /// <summary>
    /// Shows three dots vary in opacity as a typing animation
    /// </summary>
    public sealed partial class TypingIndicator : UserControl
    {
        public TypingIndicator()
        {
            this.InitializeComponent();
            Typing.Begin();
        }
    }
}
