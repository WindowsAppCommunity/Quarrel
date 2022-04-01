// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models.Handshake
{
    internal class SessionReplace
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
