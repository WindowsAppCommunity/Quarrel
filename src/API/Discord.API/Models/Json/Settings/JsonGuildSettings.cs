// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal record JsonGuildSettings
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("suppress_everyone")]
        public bool SuppressEveryone { get; set; }

        [JsonPropertyName("suppress_roles")]
        public bool SuppressRoles { get; set; }

        [JsonPropertyName("muted")]
        public bool Muted { get; set; }

        [JsonPropertyName("mobile_push")]
        public bool MobilePush { get; set; }

        // TODO: Appropriate enum values
        [JsonPropertyName("message_notifications")]
        public int MessageNotifications { get; set; }

        [JsonPropertyName("mute_config")]
        public JsonMuteConfig MuteConfig { get; set; }

        [JsonPropertyName("channel_overrides")]
        public JsonChannelOverride[] ChannelOverrides { get; set; }
    }
}
