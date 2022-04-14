// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways.Models.Handshake
{
    internal class GatewayResume
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionID { get; set; }

        [JsonPropertyName("seq")]
        public int LastSequenceNumberReceived { get; set; }
    }
}
