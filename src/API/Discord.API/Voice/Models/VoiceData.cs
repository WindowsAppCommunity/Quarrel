// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models
{
    internal class VoiceData
    {
        [JsonPropertyName("data")]
        public float[] Data { get; set; }

        [JsonPropertyName("samples")]
        public uint Samples { get; set; }
    }
}
