using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public struct GuildIntegrationUpdate
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
