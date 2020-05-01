using Newtonsoft.Json;

namespace GitHubAPI.Models
{/// <summary>
 /// A model for a contributor to a given repository
 /// </summary>
    public sealed class Contributor
    {
        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonProperty("login")]
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
        /// Gets the number of commits for the current contributor
        /// </summary>
        [JsonProperty("contributions")]
        public int CommitsCount { get; internal set; }
    }
}
