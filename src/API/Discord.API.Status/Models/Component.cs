// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    /// <summary>
    /// A component status of the Discord API.
    /// </summary>
    public partial class Component
    {
        /// <summary>
        /// Gets the status of the component.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the time the component was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets the time the component status was last updated.
        /// </summary>
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Gets the ordering position of the component.
        /// </summary>
        [JsonPropertyName("position")]
        public long Position { get; set; }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the component is a showcase.
        /// </summary>
        [JsonPropertyName("showcase")]
        public bool Showcase { get; set; }

        /// <summary>
        /// Gets the id of the component.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets the page id of the component.
        /// </summary>
        [JsonPropertyName("page_id")]
        public string PageId { get; set; }

        /// <summary>
        /// Gets the group id of the component.
        /// </summary>
        [JsonPropertyName("group_id")]
        public object GroupId { get; set; }

        /// <summary>
        /// Gets a list of child components.
        /// </summary>
        [JsonPropertyName("components")]
        public string[]? Components { get; set; }
    }
}
