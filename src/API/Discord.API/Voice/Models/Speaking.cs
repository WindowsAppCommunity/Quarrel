// Quarrel © 2022

using Discord.API.Voice.Models.Enums;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models
{
    internal class Speaking
    {
        [JsonPropertyName("speaking")]
        public SpeakingState State { get; set; }

        [JsonPropertyName("delay")]
        public int Delay { get; set; }

        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }
    }
}
