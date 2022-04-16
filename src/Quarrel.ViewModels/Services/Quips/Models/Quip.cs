// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Quarrel.Services.Quips.Models
{
    /// <summary>
    /// A quip with a creator.
    /// </summary>
    public class Quip
    {
        /// <summary>
        /// Gets text of the quip.
        /// </summary>
        [JsonPropertyName("name")]
        public string Text { get; set; }

        /// <summary>
        /// Gets the quip creator.
        /// </summary>
        [JsonPropertyName("creator")]
        public QuipCreator Creator { get; set; }
    }
}
