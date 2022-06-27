// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal record StreamWatch
    {
        [JsonPropertyName("stream_key")]
        public string StreamKey { get; set; }
    }
}
