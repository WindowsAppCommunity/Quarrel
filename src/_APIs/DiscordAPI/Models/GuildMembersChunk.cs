using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class GuildMembersChunk
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("members")]
        public IList<GuildMember> Members { get; set; }
        [JsonProperty("not_found")]
        public IList<string> NotFound { get; set; }
        [JsonProperty("presences")]
        public IList<Presence> Presences { get; set; }
    }
}
