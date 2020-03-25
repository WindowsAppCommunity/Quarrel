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

namespace Quarrel.SubPages
{
    /// <summary>
    /// Displays information about unhandled exception
    /// </summary>
    public sealed partial class BSOD : Page
    {
        /// <inheritdoc />
        public BSOD()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var exception = e.Parameter as UnhandledExceptionEventArgs;
            Message.Text = exception.Message;
        }
    }
}