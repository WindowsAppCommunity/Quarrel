// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for displaying a channel's topic.
    /// </summary>
    public sealed partial class TopicPage : UserControl, IConstrainedSubPage
    {
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicPage"/> class.
        /// </summary>
        public TopicPage()
        {
            this.InitializeComponent();

            if (SubFrameNavigationService.Parameter != null)
            {
                this.DataContext = SubFrameNavigationService.Parameter;
            }
        }

        /// <summary>
        /// Gets the displayed channel.
        /// </summary>
        public BindableChannel ViewModel => DataContext as BindableChannel;

        /// <inheritdoc/>
        public double MaxExpandedHeight => 200;

        /// <inheritdoc/>
        public double MaxExpandedWidth => 800;

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());
    }
}
