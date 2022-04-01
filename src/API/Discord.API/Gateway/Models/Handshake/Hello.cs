// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Handshake
{
    internal class Hello
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
