// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Channels;

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message indicating a channel has been deleted.
    /// </summary>
    public sealed class GatewayChannelDeletedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayChannelDeletedMessage"/> class.
        /// </summary>
        /// <param name="channel">The deleted channel.</param>
        public GatewayChannelDeletedMessage(Channel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets the deleted channel.
        /// </summary>
        public Channel Channel { get; }
    }
}
