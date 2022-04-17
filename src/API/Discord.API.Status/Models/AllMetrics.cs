// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    public class AllMetrics
    {
        [JsonPropertyName("period")]
        public Period Period { get; set; }

        [JsonPropertyName("metrics")]
        public MetricElement[] Metrics { get; set; }

        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }
    }
}
