// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake
{
    internal class SessionDescription
    {
        [JsonPropertyName("secret_key")]
        public byte[] SecretKey { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
    }
}
