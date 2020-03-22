// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page to display features added in the most recent update.
    /// </summary>
    public sealed partial class WhatsNewPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhatsNewPage"/> class.
        /// </summary>
        public WhatsNewPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 384;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 512;

        private void Close(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().GoBack();
        }
    }
}
