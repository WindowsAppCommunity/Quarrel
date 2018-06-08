using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Gateway.UpstreamEvents
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
        public Game Game { get; set; }
    }
}
