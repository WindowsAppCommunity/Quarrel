using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.UpstreamEvents
{
    public struct Speak
    {
        [JsonProperty("speaking")]
        public string Speaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
