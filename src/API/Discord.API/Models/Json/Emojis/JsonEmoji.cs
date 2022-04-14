// Quarrel © 2022

using Discord.API.Models.Json.Users;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Emojis
{
    internal class JsonEmoji
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("animated")]
        public bool? IsAnimated { get; set; }

        [JsonPropertyName("roles"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] Roles { get; set; }

        [JsonPropertyName("require_colons")]
        public bool RequireColons { get; set; }

        [JsonPropertyName("managed")]
        public bool Managed { get; set; }

        [JsonPropertyName("user")]
        public JsonUser? User { get; set; }
    }
}
