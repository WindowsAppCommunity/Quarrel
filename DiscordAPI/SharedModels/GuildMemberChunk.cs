using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.SharedModels
{
    public class GuildMemberChunk
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("members")]
        public IEnumerable<GuildMember> Members { get; set; }
    }
}
