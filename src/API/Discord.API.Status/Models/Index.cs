// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// An index of the discord api status.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Gets the index page.
        /// </summary>
        [JsonPropertyName("page")]
        public Page Page { get; set; }

        /// <summary>
        /// Gets the status for the index.
        /// </summary>
        [JsonPropertyName("status")]
        public StatusClass Status { get; set; }

        /// <summary>
        /// Gets the component statuses for the index.
        /// </summary>
        [JsonPropertyName("components")]
        public Component[] Components { get; set; }

        /// <summary>
        /// Gets the index incidents.
        /// </summary>
        [JsonPropertyName("incidents")]
        public Incident[] Incidents { get; set; }
    }
}
