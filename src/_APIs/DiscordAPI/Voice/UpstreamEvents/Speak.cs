using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Voice.UpstreamEvents
{
    public class Speak
    {
        [JsonProperty("speaking")]
        public bool Speaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
    }
}
