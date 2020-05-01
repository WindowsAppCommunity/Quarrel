using Newtonsoft.Json;
using System;

namespace DiscordStatusAPI.Models
{
    public partial class Component
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("position")]
        public long Position { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("showcase")]
        public bool Showcase { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("page_id")]
        public string PageId { get; set; }

        [JsonProperty("group_id")]
        public object GroupId { get; set; }

        [JsonProperty("components", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Components { get; set; }
    }
}
