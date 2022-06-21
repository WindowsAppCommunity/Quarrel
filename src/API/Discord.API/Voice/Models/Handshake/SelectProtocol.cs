// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models.Handshake
{
    internal record SelectProtocol<T>
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("codecs")]
        public Codec[] Codecs { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
        
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("mode")]
        public string Mode { get; set; }
    }
}
