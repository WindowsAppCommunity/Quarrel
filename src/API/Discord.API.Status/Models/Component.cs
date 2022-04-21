// Quarrel © 2022

using System;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Status.Models
{
    public partial class Component
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonPropertyName("position")]
        public long Position { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("showcase")]
        public bool Showcase { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("page_id")]
        public string PageId { get; set; }

        [JsonPropertyName("group_id")]
        public object GroupId { get; set; }

        [JsonPropertyName("components")]
        public string[]? Components { get; set; }
    }
}
