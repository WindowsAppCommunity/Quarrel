using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class GuildMemberUpdate
    {
        [JsonProperty("guild_id")]
        public string guildId { get; set; }
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("nick")]
        public string Nick { get; set; }
    }
}
