// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake
{
    internal record VoiceSessionDescription
    {
        [JsonPropertyName("audio_codec")]
        public string? AudioCodec { get; set; }
         
        [JsonPropertyName("media_session_id")]
        public string MediaSessionId { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
        
        [JsonPropertyName("secret_key")]
        public short[] SecretKey { get; set; }
        
        [JsonPropertyName("video_codec")]
        public string? VideoCodec { get; set; }
    }
}
