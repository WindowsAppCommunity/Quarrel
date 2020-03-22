// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Gateway.DownstreamEvents;

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message to indicate a session is invalid.
    /// </summary>
    public sealed class GatewayInvalidSessionMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayInvalidSessionMessage"/> class.
        /// </summary>
        /// <param name="packet">Invalid session data.</param>
        public GatewayInvalidSessionMessage(InvalidSession packet)
        {
            EventData = packet;
        }

        /// <summary>
        /// Gets the Invalid session data.
        /// </summary>
        public InvalidSession EventData { get; }
    }
}
