// Quarrel © 2022

using Discord.API.Models.Enums.Guilds;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Guilds
{
    internal record JsonModifyGuild
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("verification_level")]
        public VerificationLevel? VerificationLevel { get; set; }

        [JsonPropertyName("afk_channel_id")]
        public ulong? AfkChannelId { get; set; }

        [JsonPropertyName("afk_timeout")]
        public int? AfkTimeout { get; set; }
    }
}
