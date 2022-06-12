// Quarrel © 2022

using Discord.API.Sockets;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways
{
    internal class GatewaySocketFrame
    {
        [JsonPropertyName("op")]
        public GatewayOperation Operation { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public GatewayEvent? Event { get; set; }
    }
}
