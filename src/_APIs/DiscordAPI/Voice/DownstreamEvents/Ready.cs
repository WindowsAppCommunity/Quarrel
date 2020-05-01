using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class Ready
    {
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("modes")]
        public IEnumerable<string> Modes { get; set; }
    }
}