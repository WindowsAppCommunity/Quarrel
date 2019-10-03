using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;

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
