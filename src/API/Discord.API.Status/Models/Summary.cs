// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// A summary of a data set.
    /// </summary>
    public partial class Summary
    {
        /// <summary>
        /// The sum of the data.
        /// </summary>
        [JsonPropertyName("sum")]
        public double Sum { get; set; }

        /// <summary>
        /// The mean of the data.
        /// </summary>
        [JsonPropertyName("mean")]
        public double Mean { get; set; }
    }
}
