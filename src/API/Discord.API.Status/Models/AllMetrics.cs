// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// The set of all response time metrics over a period.
    /// </summary>
    public class AllMetrics
    {
        /// <summary>
        /// Gets the peroid of time the metrics cover.
        /// </summary>
        [JsonPropertyName("period")]
        public Period Period { get; set; }

        /// <summary>
        /// Gets the set of individual response time elements.
        /// </summary>
        [JsonPropertyName("metrics")]
        public MetricElement[] Metrics { get; set; }

        /// <summary>
        /// Gets the summary of the response time.
        /// </summary>
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }
    }
}
