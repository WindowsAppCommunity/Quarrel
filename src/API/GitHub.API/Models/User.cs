// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace GitHub.API.Models
{
    /// <summary>
    /// A model for a GitHub user
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonPropertyName("login")]
        public string Username { get; set; }

        /// <summary>
        /// Gets the URL of the contributor profile image
        /// </summary>
        [JsonPropertyName("avatar_url")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Gets the URL of the contributor profile page
        /// </summary>
        [JsonPropertyName("html_url")]
        public string ProfilePageUrl { get; set; }

        /// <summary>
        /// Gets the user bio
        /// </summary>
        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        /// <summary>
        /// Name or username if null
        /// </summary>
        [JsonIgnore]
        public string DisplayName => Name ?? Username;
    }
}
