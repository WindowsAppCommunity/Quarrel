// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Roles
{
    internal class JsonRole
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("unicode_emoji")]
        public string? Emoji { get; set; }

        [JsonPropertyName("color")]
        public uint? Color{ get; set; }

        [JsonPropertyName("hoist")]
        public bool Hoist { get; set; }

        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("permissions"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Permissions { get; set; }

        [JsonPropertyName("managed")]
        public bool Managed { get; set; }

        [JsonPropertyName("tags")]
        public FullRoleTags Tags { get; set; }
    }
}
