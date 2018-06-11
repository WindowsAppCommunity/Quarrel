using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_UWP.SharedModels;

namespace Discord_UWP.SharedModels
{
    public class GuildSync
    {
        [JsonProperty("id")]
        public string GuildId { get; set; }
        [JsonProperty("large")]
        public bool IsLarge { get; set; }
        [JsonProperty("members")]
        public IEnumerable<GuildMember> Members { get; set; }
        [JsonProperty("presences")]
        public IEnumerable<Presence> Presences { get; set; }

    }
}
