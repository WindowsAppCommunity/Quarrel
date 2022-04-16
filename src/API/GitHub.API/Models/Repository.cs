// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace GitHub.API.Models
{
    internal class Repository
    {
        /// <summary>
        /// Gets the repository id.
        /// </summary>
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Gets the repository' name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the repository's full name.
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets whether or not the reposity is private.
        /// </summary>
        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }
    }
}
