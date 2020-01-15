using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;

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
