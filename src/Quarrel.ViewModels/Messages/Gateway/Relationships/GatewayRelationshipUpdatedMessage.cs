// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway.Relationships
{
    /// <summary>
    /// A message indicating an updated relationship.
    /// </summary>
    public class GatewayRelationshipUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayRelationshipUpdatedMessage"/> class.
        /// </summary>
        /// <param name="friend">The relationship data.</param>
        public GatewayRelationshipUpdatedMessage(Friend friend)
        {
            Friend = friend;
        }

        /// <summary>
        /// Gets the updated friend.
        /// </summary>
        public Friend Friend { get; }
    }
}
