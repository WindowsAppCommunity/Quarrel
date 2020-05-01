using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class TypingStart
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
    }
}
