// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake
{
    internal record VoiceReady
    {
        internal record Stream
        {
            [JsonPropertyName("active")]
            public bool Active { get; set; }

            [JsonPropertyName("quality")]
            public int Quality { get; set; }

            [JsonPropertyName("rid")]
            public string Rid { get; set; }

            [JsonPropertyName("rtx_ssrc")]
            public uint RtxSSRC { get; set; }

            [JsonPropertyName("ssrc")]
            public uint SSRC { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }
        
        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("modes")]
        public string[] Modes { get; set; }

        [JsonPropertyName("streams")]
        public Stream[] Streams { get; set; }
    }
}
