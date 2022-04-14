// Quarrel © 2022

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Gateways
{
    internal class SocketFrame
    {
        [JsonPropertyName("op")]
        public OperationCode? Operation { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public string Type { get; set; }
    }
    internal class SocketFrame<T> : SocketFrame
    {
        [JsonPropertyName("d")]
        public T Payload { get; set; }
    }
}
