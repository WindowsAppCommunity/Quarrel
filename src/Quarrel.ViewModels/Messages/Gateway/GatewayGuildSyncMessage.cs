using DiscordAPI.Models;
using System.Collections.Generic;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayGuildSyncMessage
    {
        public GatewayGuildSyncMessage(string guildId, List<GuildMember> members)
        {
            GuildId = guildId;
            Members = members;
        }

        public string GuildId { get; }
        public List<GuildMember> Members { get; }
    }
}
