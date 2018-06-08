using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public class GuildBanUpdate
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
