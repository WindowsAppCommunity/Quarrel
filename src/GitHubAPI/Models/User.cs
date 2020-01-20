using Newtonsoft.Json;

namespace GitHubAPI.Models
{
    /// <summary>
    /// A model for a GitHub user
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile image
        /// </summary>
        [JsonProperty("avatar_url")]
        public string ProfileImageUrl { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile page
        /// </summary>
        [JsonProperty("html_url")]
        public string ProfilePageUrl { get; internal set; }

        /// <summary>
        /// Gets the user bio
        /// </summary>
        [JsonProperty("bio")]
        public string Bio { get; internal set; }
    }
}
