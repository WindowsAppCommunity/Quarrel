// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Handshake
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
