using Newtonsoft.Json;

namespace DiscordAPI.Gateway.UpstreamEvents
{
    public class GatewayResume
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public int LastSequenceNumberReceived { get; set; }
    }
}
