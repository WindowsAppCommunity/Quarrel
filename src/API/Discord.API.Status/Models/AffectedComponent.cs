// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// A component that is affected by an issue.
    /// </summary>
    public class AffectedComponent
    {
        /// <summary>
        /// Gets the code of the issue that altered the component status.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets the name of the comonent.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the status of the component before the issue.
        /// </summary>
        [JsonPropertyName("old_status")]
        public string OldStatus { get; set; }

        /// <summary>
        /// Gets the status of the component with the issue.
        /// </summary>
        [JsonPropertyName("new_status")]
        public string NewStatus { get; set; }
    }
}
