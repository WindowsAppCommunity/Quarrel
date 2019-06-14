using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Voice.DownstreamEvents
{
    public class Speak
    {
        [JsonProperty("speaking")]
        public bool Speaking { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("ssrc")]
        public int SSRC { get; set; }
    }
}
