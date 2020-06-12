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
        private ISubFrameNavigationService _subFrameNavigationService = null;

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

        /// <summary>
        /// Gets a value indicating whether or not to show the insider label.
        /// </summary>
        public bool IsInsider => App.IsInsiderBuild;

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        private void Close(object sender, RoutedEventArgs e)
        {
            SubFrameNavigationService.GoBack();
        }
    }
}
