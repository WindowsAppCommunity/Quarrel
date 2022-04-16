// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace GitHub.API.Models
{
    /// <summary>
    /// A model for a contributor to a given repository
    /// </summary>
    public sealed class Contributor
    {
        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonPropertyName("login")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile image
        /// </summary>
        [JsonPropertyName("avatar_url")]
        public string ProfileImageUrl { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile page
        /// </summary>
        [JsonPropertyName("html_url")]
        public string ProfilePageUrl { get; internal set; }

        /// <summary>
        /// Gets the number of commits for the current contributor
        /// </summary>
        [JsonPropertyName("contributions")]
        public int CommitsCount { get; internal set; }
    }
}
