using Newtonsoft.Json;

namespace Discord_UWP.Voice.UpstreamEvents
{
    public struct Identify
    {
        [JsonProperty("server_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
