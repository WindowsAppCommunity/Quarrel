using System;
using System.Collections.Generic;
using System.Text;
using DiscordAPI.Models;

namespace Quarrel.ViewModels.ViewModels.Messages.Gateway
{
    public class GatewayGuildMembersChunkMessage
    {
        public GuildMembersChunk GuildMembersChunk { get; set; }

        public GatewayGuildMembersChunkMessage(GuildMembersChunk guildMembersChunk)
        {
            GuildMembersChunk = guildMembersChunk;
        }
    }
}
