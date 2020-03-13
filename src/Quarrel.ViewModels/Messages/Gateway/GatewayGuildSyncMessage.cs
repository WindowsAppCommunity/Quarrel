// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message indicating a recieved guild sync.
    /// </summary>
    public sealed class GatewayGuildSyncMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayGuildSyncMessage"/> class.
        /// </summary>
        /// <param name="guildId">The id of the guild.</param>
        /// <param name="members">The members synced.</param>
        public GatewayGuildSyncMessage(string guildId, List<GuildMember> members)
        {
            GuildId = guildId;
            Members = members;
        }

        /// <summary>
        /// Gets the id of guild.
        /// </summary>
        public string GuildId { get; }

        /// <summary>
        /// Gets the members synced.
        /// </summary>
        public List<GuildMember> Members { get; }
    }
}
