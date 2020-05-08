// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Guilds;

namespace Quarrel.ViewModels.Messages.Navigation
{
    /// <summary>
    /// A message to request changing channels.
    /// </summary>
    public sealed class ChannelNavigateMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelNavigateMessage"/> class.
        /// </summary>
        /// <param name="channel">The channel to navigate to.</param>
        /// <param name="guild">The guild to navigate to.</param>
        public ChannelNavigateMessage(BindableChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets the channel to navigate to.
        /// </summary>
        public BindableChannel Channel { get; }

        /// <summary>
        /// Gets the guild to navigate to.
        /// </summary>
        public BindableGuild Guild => Channel.Guild;
    }
}
