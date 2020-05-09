using Newtonsoft.Json;

namespace DiscordAPI.Voice.UpstreamEvents
{
    public class Identify
    {
        [JsonProperty("server_id")]
        public string ServerId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("video")]
        public bool Video { get; set; }
    }
}
