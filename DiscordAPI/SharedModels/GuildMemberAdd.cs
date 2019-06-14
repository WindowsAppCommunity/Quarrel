using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class GuildMemberAdd
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("nick")]
        public string Nick { get; set; }
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }
        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
        [JsonProperty("mute")]
        public bool Mute { get; set; }
        [JsonProperty("guild_id")]
        public string guildId { get; set; }
    }
}
