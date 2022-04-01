// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal class VoiceStatusUpdate
    {
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }

        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }
        
        [JsonPropertyName("self_mute")]
        public bool Mute { get; set; }
        
        [JsonPropertyName("self_deaf")]
        public bool Deaf { get; set; }
    }
}
