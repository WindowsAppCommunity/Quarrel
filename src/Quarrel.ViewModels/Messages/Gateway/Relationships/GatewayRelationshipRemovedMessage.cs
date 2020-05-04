// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway.Relationships
{
    /// <summary>
    /// A message indicating a removed relationship.
    /// </summary>
    public class GatewayRelationshipRemovedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayRelationshipRemovedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public GatewayRelationshipRemovedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the removed friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
