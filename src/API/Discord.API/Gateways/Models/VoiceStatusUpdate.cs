// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models
{
    internal class VoiceStatusUpdate
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("channel_id"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
        public ulong? ChannelId { get; set; }
        
        [JsonPropertyName("self_mute")]
        public bool Mute { get; set; }
        
        [JsonPropertyName("self_deaf")]
        public bool Deaf { get; set; }
    }
}
