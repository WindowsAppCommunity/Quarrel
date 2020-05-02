using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class Resumed
    {
        [JsonProperty("_trace")]
        public IEnumerable<string> Trace { get; set; }
    }
}
