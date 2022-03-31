// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Rest.Models.Roles
{
    internal class RestRole
    {
        [JsonPropertyName("id")]
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
        public bool Hoise { get; set; }

        [JsonPropertyName("mentionable")]
        public bool Mentionable { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("permissions")]
        public uint Permissions { get; set; }

        [JsonPropertyName("managed")]
        public bool Managed { get; set; }

        [JsonPropertyName("tags")]
        public RestRoleTags Tags { get; set; }
    }
}
