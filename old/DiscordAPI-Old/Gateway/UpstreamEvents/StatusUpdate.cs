using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Gateway.UpstreamEvents
{

    public struct StatusUpdate
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("since")]
        public long? IdleSince { get; set; }
        [JsonProperty("afk")]
        public bool IsAFK { get; set; }
        [JsonProperty("game")]
        public GameBase Game { get; set; }
    }
}
