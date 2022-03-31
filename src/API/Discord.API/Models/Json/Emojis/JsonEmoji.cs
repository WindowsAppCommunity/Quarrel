// Adam Dernis © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Emojis
{
    internal class JsonEmoji
    {
        [JsonPropertyName("id")]
        public ulong? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("animated")]
        public bool? IsAnimated { get; set; }

        [JsonPropertyName("roles")]
        public ulong[] Roles { get; set; }

        [JsonPropertyName("require_colons")]
        public bool RequireColons { get; set; }

        [JsonPropertyName("managed")]
        public bool Managed { get; set; }

        [JsonPropertyName("user")]
        public JsonUser? User { get; set; }
    }
}
