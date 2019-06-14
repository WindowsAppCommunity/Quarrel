using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway.DownstreamEvents
{
    public class GuildDelete
    {
        [JsonProperty("id")]
        public string GuildId { get; set; }
        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }
    }
}
