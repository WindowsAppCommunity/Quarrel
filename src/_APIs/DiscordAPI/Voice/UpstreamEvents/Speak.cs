using Newtonsoft.Json;

namespace DiscordAPI.Voice.UpstreamEvents
{
    public class Speak
    {
        [JsonProperty("speaking")]
        public int Speaking { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
    }
}
