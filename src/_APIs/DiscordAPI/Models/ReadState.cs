using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class ReadState
    {
        [JsonProperty("last_pin_timestamp")]
        public string LastPinTimestamp { get; set; }
        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("mention_count")]
        public int MentionCount { get; set; }
    }
}
