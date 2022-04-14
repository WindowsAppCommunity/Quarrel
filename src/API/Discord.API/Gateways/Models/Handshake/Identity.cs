// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Handshake
{
    internal class Identity
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        
        [JsonPropertyName("properties")]
        public IdentityProperties Properties { get; set; }
        
        [JsonPropertyName("compress")]
        public bool Compress { get; set; }
        
        [JsonPropertyName("large_threshold")]
        public int LargeThreshold { get; set; }
    }
}
