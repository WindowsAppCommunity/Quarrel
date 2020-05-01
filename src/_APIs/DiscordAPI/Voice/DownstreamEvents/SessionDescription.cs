using Newtonsoft.Json;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class SessionDescription
    {
        [JsonProperty("secret_key")]
        public byte[] SecretKey { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}