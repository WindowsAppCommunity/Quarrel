// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake.Identity
{
    internal record VoiceIdentity
    {
        [JsonPropertyName("server_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong ServerId { get; set; }

        [JsonPropertyName("user_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong UserId { get; set; }

        [JsonPropertyName("session_id")]
        public string? SessionId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }
    }
}
