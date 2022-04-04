// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages.Embeds
{
    internal class JsonEmbed
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("footer")]
        public JsonEmbedFooter Footer { get; set; }

        [JsonPropertyName("image")]
        public JsonEmbedMedia Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public JsonEmbedMedia Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public JsonEmbedMedia Video { get; set; }

        [JsonPropertyName("provider")]
        public JsonEmbedProvider Provider { get; set; }

        [JsonPropertyName("author")]
        public JsonEmbedAuthor Author { get; set; }

        [JsonPropertyName("fields")]
        public JsonEmbedField[] Fields { get; set; }
    }
}
