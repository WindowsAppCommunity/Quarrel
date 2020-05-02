using DiscordAPI.Models;
using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class MessageReactionUpdate
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }

    public class MessageReactionRemoveAll
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
    }
}
