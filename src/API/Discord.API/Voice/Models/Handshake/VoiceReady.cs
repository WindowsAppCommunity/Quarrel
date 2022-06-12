// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake
{
    internal class VoiceReady
    {
        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("modes")]
        public string[] Modes { get; set; }
    }
}
