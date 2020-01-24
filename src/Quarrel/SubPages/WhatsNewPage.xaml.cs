using GalaSoft.MvvmLight.Ioc;
using Quarrel.Navigation;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Navigation;
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
    public sealed partial class WhatsNewPage : UserControl, IConstrainedSubPage
    {
        public WhatsNewPage()
        {
            this.InitializeComponent();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().GoBack();
        }

        #region IConstrainedSubPage

        public double MaxExpandedHeight { get; } = 384;
        public double MaxExpandedWidth { get; } = 512;

        #endregion
    }
}
