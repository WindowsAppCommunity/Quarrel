// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// The period of time a metrics set covers.
    /// </summary>
    public partial class Period
    {
        /// <summary>
        /// Gets the number of metrics in the peroid.
        /// </summary>
        [JsonPropertyName("count")]
        public long Count { get; set; }

        /// <summary>
        /// Gets the interval of time between metric.
        /// </summary>
        [JsonPropertyName("interval")]
        public long Interval { get; set; }

        /// <summary>
        /// Gets the interval identifier.
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
    }
}
