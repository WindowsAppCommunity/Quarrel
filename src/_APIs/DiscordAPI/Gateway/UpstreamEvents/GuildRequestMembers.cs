using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.UpstreamEvents
{
    public class GuildRequestMembers
    {
        [JsonProperty("guild_id")]
        public IEnumerable<string> GuildIds { get; set; }
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("limit")]
        public int? Limit { get; set; }
        [JsonProperty("presences")]
        public bool? Presences { get; set; }
        [JsonProperty("user_ids")]
        public IEnumerable<string> UserIds { get; set; }
    }
}
