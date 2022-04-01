// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Handshake
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
