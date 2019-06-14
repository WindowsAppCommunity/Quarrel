using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Presence
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set;}
        [JsonProperty("game")]
        public Game Game { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
