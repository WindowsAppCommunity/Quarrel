// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Handshake
{
    internal class IdentityProperties
    {
        [JsonPropertyName("$os")]
        public string OS { get; set; }
        
        [JsonPropertyName("$browser")]
        public string Browser { get; set; }
        
        [JsonPropertyName("$device")]
        public string Device { get; set; }
        
        [JsonPropertyName("$referrer")]
        public string Referrer { get; set; }
        
        [JsonPropertyName("$referring_domain")]
        public string ReferringDomain { get; set; }
    }
}
