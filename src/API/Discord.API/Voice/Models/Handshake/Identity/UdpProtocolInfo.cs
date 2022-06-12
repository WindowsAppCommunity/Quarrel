// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake.Identity
{
    internal record UdpProtocolInfo
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("codecs")]
        public Codec[] Codecs { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
    }
}
