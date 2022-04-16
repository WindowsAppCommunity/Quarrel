// Quarrel © 2022

using Quarrel.Services.Quips.Enums;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Quarrel.Services.Quips.Models
{
    /// <summary>
    /// The creator of a quip.
    /// </summary>
    public class QuipCreator
    {
        /// <summary>
        /// The source for the creator handler.
        /// </summary>
        [JsonPropertyName("source")]
        public CreatorSource Source { get; set; }

        /// <summary>
        /// The name of the creator.
        /// </summary>
        [JsonPropertyName("name")]
        public string? CreatorName { get; set; }

        /// <summary>
        /// Gets the creator's handle with whatever source it's from.
        /// </summary>
        /// <remarks>
        /// Twitter: @Handle
        /// Discord: Handle#1234
        /// etc
        /// </remarks>
        [JsonPropertyName("handle")]
        public string CreatorHandle { get; set;}

        /// <summary>
        /// Gets the name to use to display the creator.
        /// </summary>
        public string CreatorDisplayName => CreatorName ?? CreatorHandle;
    }
}
