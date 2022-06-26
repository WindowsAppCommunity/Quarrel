// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Discord.API.Gateways.Models
{
    internal record StreamCreate
    {
        [JsonPropertyName("viewer_ids")]
        public string[] ViewerIds { get; set; }

        [JsonPropertyName("stream_key")]
        public string StreamKey { get; set; }

        [JsonPropertyName("rtc_server_id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong RtcServerId { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("paused")]
        public bool Paused { get; set; }
    }
}
