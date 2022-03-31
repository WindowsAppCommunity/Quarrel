// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages.Embeds
{
    internal class JsonEmbedField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("inline")]
        public bool Inline { get; set; }
    }
}
