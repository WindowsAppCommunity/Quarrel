// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway.Relationships
{
    /// <summary>
    /// A message indicating a new relationship.
    /// </summary>
    public class GatewayRelationshipAddedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayRelationshipAddedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public GatewayRelationshipAddedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the new friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
