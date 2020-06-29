// Copyright (c) Quarrel. All rights reserved.

using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.SubPages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for changing a user's nickanme in a guild.
    /// </summary>
    public sealed partial class ChangeNicknamePage : UserControl, IConstrainedSubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNicknamePage"/> class.
        /// </summary>
        public ChangeNicknamePage()
        {
            this.InitializeComponent();
            this.DataContext = new ChangeNicknameViewModel();
        }

        /// <summary>
        /// Gets the new channel data.
        /// </summary>
        public ChangeNicknameViewModel ViewModel => this.DataContext as ChangeNicknameViewModel;

        /// <inheritdoc/>
        public double MaxExpandedHeight { get; } = 300;

        /// <inheritdoc/>
        public double MaxExpandedWidth { get; } = 400;
    }
}
