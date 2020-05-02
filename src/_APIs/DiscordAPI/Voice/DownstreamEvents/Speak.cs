using Newtonsoft.Json;

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
