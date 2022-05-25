// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Settings
{
    internal class JsonModifyUserSettings
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("theme")]
        public string? Theme { get; set; }

        [JsonPropertyName("restricted_guilds")]
        public ulong[]? RestrictedGuilds { get; set; }
    }
}
