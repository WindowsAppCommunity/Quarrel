// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Status.Models
{
    /// <summary>
    /// A response time datum.
    /// </summary>
    public partial class Datum
    {
        /// <summary>
        /// Gets the timestamp represented.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets the average response time during this time.
        /// </summary>
        [JsonPropertyName("value")]
        public ushort Value { get; set; }
    }
}
