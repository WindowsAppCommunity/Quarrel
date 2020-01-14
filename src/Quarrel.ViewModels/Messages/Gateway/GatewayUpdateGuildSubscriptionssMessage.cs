using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public class GatewayUpdateGuildSubscriptionssMessage
    {
        public IReadOnlyDictionary<string, IEnumerable<int[]>> Channels { get; set; }
        public string GuildId { get; set; }

        public GatewayUpdateGuildSubscriptionssMessage(string guildId, IReadOnlyDictionary<string, IEnumerable<int[]>> channels)
        {
            Channels = channels;
            GuildId = guildId;
        }
    }
}
