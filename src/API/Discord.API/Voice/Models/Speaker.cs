// Quarrel © 2022

using Discord.API.Voice.Models.Enums;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models
{
    internal record Speaker
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }

        [JsonPropertyName("speaking")]
        public int IsSpeaking { get; set; }
    }
}
