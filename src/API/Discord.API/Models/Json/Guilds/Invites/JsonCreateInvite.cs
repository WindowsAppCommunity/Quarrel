// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Guilds.Invites
{
    internal record JsonCreateInvite
    {
        [JsonPropertyName("max_age")]
        public int MaxAge { get; set; }

        [JsonPropertyName("max_uses")]
        public int MaxUses { get; set; }

        [JsonPropertyName("temporary")]
        public bool Temporary { get; set; }

        [JsonPropertyName("unique")]
        public bool Unique { get; set; }
    }
}
