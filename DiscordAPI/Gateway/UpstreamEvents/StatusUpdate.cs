using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway.UpstreamEvents
{

    public struct StatusUpdate
    {

        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("idle_since")]
        public int? IdleSince { get; set; }
        [JsonProperty("game")]
        public Discord_UWP.SharedModels.Game? Game { get; set; }
    }
}
