using System.Collections.Generic;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public class GatewayUpdateGuildSubscriptionsMessage
    {
        public IReadOnlyDictionary<string, IEnumerable<int[]>> Channels { get; set; }
        public IEnumerable<string> Members { get; set; }
        public string GuildId { get; set; }

        public GatewayUpdateGuildSubscriptionsMessage(string guildId, IReadOnlyDictionary<string, IEnumerable<int[]>> channels)
        {
            Channels = channels;
            GuildId = guildId;
        }
        public GatewayUpdateGuildSubscriptionsMessage(string guildId, IEnumerable<string> members = null)
        {
            GuildId = guildId;
            Members = members;
        }
    }
}
