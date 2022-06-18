// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace GitHub.API.Models
{
    /// <summary>
    /// A model for a GitHub release
    /// </summary>
    public class Release
    {
        /// <summary>
        /// Gets the release id.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets url of the release.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets author of the release.
        /// </summary>
        [JsonPropertyName("author")]
        public Contributor Author { get; set; }

        /// <summary>
        /// Gets tag name for the release.
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        /// <summary>
        /// Gets the name of the release.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the release is a draft.
        /// </summary>
        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the release is a prerelease.
        /// </summary>
        [JsonPropertyName("prerelease")]
        public bool IsPrerelease { get; set; }
    }
}
