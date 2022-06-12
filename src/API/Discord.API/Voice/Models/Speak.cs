// Quarrel © 2022

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Voice.Models
{
    internal class Speak
    {
        [JsonPropertyName("speaking")]
        public int Speaking { get; set; }

        [JsonPropertyName("delay")]
        public int Delay { get; set; }

        [JsonPropertyName("ssrc")]
        public uint SSRC { get; set; }
    }
}
