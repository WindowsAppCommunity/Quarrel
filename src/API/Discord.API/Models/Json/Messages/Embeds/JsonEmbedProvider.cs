// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages.Embeds
{
    internal class JsonEmbedProvider
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
