using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class Reaction
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("me")]
        public bool Me { get; set; }

        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonIgnore]
        public string MessageId { get; set; }

        [JsonIgnore]
        public string ChannelId { get; set; }
    }
}
