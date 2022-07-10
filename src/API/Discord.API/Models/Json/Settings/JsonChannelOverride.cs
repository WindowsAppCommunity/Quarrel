// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal record JsonChannelOverride
    {
        [JsonPropertyName("channel_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ChannelId { get; set; }

        [JsonPropertyName("muted")]
        public bool Muted { get; set; }

        [JsonPropertyName("mute_config")]
        public JsonMuteConfig MuteConfig { get; set; }

        // TODO: Appropriate enum values
        [JsonPropertyName("message_notifications")]
        public int MessageNotifications { get; set; }
        
        [JsonPropertyName("collapsed")]
        public bool Collapsed { get; set; }
    }
}
