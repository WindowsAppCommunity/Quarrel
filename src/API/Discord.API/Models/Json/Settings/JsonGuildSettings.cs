// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Settings
{
    internal class JsonGuildSettings
    {
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }

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
