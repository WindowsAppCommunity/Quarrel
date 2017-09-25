using Discord_UWP.SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.DownstreamEvents
{
    public struct Hello
    {
        [JsonProperty("heartbeat_interval")]
        public int Heartbeatinterval { get; set; }
    }
}