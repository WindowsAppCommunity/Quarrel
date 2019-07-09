using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class Speak
    {
        [JsonProperty("speaking")]
        public int Speaking { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("ssrc")]
        public int SSRC { get; set; }
    }
}
