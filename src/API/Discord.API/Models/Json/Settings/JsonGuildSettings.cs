// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal class JsonGuildSettings
    {
        [JsonPropertyName("guild_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? GuildId { get; set; }

        [JsonPropertyName("suppress_everyone")]
        public bool SuppressEveryone { get; set; }

        [JsonPropertyName("muted")]
        public bool Muted { get; set; }

        [JsonPropertyName("mobile_push")]
        public bool MobilePush { get; set; }

        [JsonPropertyName("message_notifications")]
        public int MessageNotifications { get; set; }
    }
}
