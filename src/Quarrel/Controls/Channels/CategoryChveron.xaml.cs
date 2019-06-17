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
using Microsoft.Toolkit.Uwp.UI.Animations;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Channels
{
    public sealed partial class CategoryChveron : UserControl
    {
        public CategoryChveron()
        {
            this.InitializeComponent();
        }

        private bool _IsCollapsed;

        public bool IsCollapsed
        {
            get => _IsCollapsed;
            set
            {
                if (value)
                    Chevron.Rotate(-90, 7, 7, 400, 0, EasingType.Circle).Start();
                else
                    Chevron.Rotate(0, 7, 7, 400, 0, EasingType.Circle).Start();

                _IsCollapsed = value;
            }
        }
    }
}
