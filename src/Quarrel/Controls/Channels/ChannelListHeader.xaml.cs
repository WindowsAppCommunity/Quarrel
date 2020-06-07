// Copyright (c) Quarrel. All rights reserved.

using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Channels
{
    /// <summary>
    /// Guild Button as header for ChannelList.
    /// </summary>
    public sealed partial class ChannelListHeader : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelListHeader"/> class.
        /// </summary>
        public ChannelListHeader()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the current Guild.
        /// </summary>
        public BindableGuild ViewModel => DataContext as BindableGuild;
    }
}
