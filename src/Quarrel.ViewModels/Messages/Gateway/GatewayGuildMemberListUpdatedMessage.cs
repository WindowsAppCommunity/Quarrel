// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message indicating an update to the member list.
    /// </summary>
    public class GatewayGuildMemberListUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayGuildMemberListUpdatedMessage"/> class.
        /// </summary>
        /// <param name="guildMemberListUpdated">The member list update object.</param>
        public GatewayGuildMemberListUpdatedMessage(GuildMemberListUpdated guildMemberListUpdated)
        {
            GuildMemberListUpdated = guildMemberListUpdated;
        }

        /// <summary>
        /// Gets the member list update details.
        /// </summary>
        public GuildMemberListUpdated GuildMemberListUpdated { get; private set; }
    }
}
