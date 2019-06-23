using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayGuildSyncMessage
    {
        public GatewayGuildSyncMessage(string guildId)
        {
            GuildId = guildId;
        }

        public string GuildId { get; }
    }
}
