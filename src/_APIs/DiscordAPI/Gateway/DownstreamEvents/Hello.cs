using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class Hello
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
        [JsonProperty("_trace")]
        public IEnumerable<string> Trace { get; set; }
    }
}
