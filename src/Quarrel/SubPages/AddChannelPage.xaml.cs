// Copyright (c) Quarrel. All rights reserved.

using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for adding channels to a guild.
    /// </summary>
    public sealed partial class AddChannelPage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddChannelPage"/> class.
        /// </summary>
        public AddChannelPage()
        {
            this.InitializeComponent();
            this.DataContext = new AddChannelPageViewModel();
        }

        /// <summary>
        /// Gets the new channel data.
        /// </summary>
        public AddChannelPageViewModel ViewModel => this.DataContext as AddChannelPageViewModel;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 300;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 400;
    }
}
