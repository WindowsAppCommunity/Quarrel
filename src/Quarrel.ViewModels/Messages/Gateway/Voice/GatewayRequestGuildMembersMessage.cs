using System.Collections.Generic;

namespace Quarrel.ViewModels.Messages.Gateway.Voice
{
    public sealed class GatewayRequestGuildMembersMessage
    {
        public IEnumerable<string> GuildIds { get; set; }
        public string Query { get; set; }
        public int? Limit { get; set; }
        public bool? Presences { get; set; }
        public IEnumerable<string> UserIds { get; set; }

        public GatewayRequestGuildMembersMessage(IEnumerable<string> guildIds, string query, int? limit, bool presences, IEnumerable<string> userIds)
        {
            GuildIds = guildIds;
            Query = query;
            Limit = limit;
            Presences = presences;
            UserIds = userIds;
        }
        public GatewayRequestGuildMembersMessage(IEnumerable<string> guildIds, IEnumerable<string> userIds)
        {
            GuildIds = guildIds;
            UserIds = userIds;
        }
    }
}
