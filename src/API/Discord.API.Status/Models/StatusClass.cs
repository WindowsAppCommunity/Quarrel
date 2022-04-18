// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public partial class StatusClass
    {
        [JsonPropertyName("indicator")]
        public string Indicator { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
