// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Gateway.DownstreamEvents;

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message indicating a Ready packet was recievied.
    /// </summary>
    public sealed class GatewayReadyMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayReadyMessage"/> class.
        /// </summary>
        /// <param name="packet">The ready packet.</param>
        public GatewayReadyMessage(Ready packet)
        {
            EventData = packet;
        }

        /// <summary>
        /// Gets the ready packets.
        /// </summary>
        public Ready EventData { get; private set; }
    }
}
