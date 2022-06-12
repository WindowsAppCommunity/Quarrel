// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways
{
    internal class UnknownOperationGatewaySocketFrame : GatewaySocketFrame
    {
        [JsonPropertyName("op")]
        public new int Operation { get; set; }
    }
}
