// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public partial class MetricElement
    {
        [JsonPropertyName("metric")]
        public Metric Metric { get; set; }

        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }

        [JsonPropertyName("data")]
        public Datum[] Data { get; set; }
    }
}
