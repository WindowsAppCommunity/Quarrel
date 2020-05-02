using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class MessageDelete
    {
        [JsonProperty("id")]
        public string MessageId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
