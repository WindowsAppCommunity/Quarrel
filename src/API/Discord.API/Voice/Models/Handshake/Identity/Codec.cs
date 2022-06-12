// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake.Identity
{
    internal record Codec
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("payloadType")]
        public int PayloadType { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("rtx_payload_type")]
        public int RtxPayloadType { get; set; }
    }
}
