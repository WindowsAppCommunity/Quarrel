// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;

namespace Quarrel.ViewModels.ViewModels.Messages.Gateway
{
    /// <summary>
    /// A message indicating a recieved guild member chunk.
    /// </summary>
    public class GatewayGuildMembersChunkMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayGuildMembersChunkMessage"/> class.
        /// </summary>
        /// <param name="guildMembersChunk">Guild Member chunk data.</param>
        public GatewayGuildMembersChunkMessage(GuildMembersChunk guildMembersChunk)
        {
            GuildMembersChunk = guildMembersChunk;
        }

        /// <summary>
        /// Gets member chunk data.
        /// </summary>
        public GuildMembersChunk GuildMembersChunk { get; private set; }
    }
}
