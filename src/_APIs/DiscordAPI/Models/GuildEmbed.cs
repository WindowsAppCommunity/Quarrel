using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class GuildEmbed
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
