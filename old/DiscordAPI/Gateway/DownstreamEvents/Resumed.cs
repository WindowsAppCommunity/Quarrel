using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway.DownstreamEvents
{
    public class Resumed
    {
        [JsonProperty("_trace")]
        public IEnumerable<string> Trace { get; set; }
    }
}
