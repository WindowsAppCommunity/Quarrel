// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Settings
{
    internal class JsonGuildFolder
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("guild_ids")]
        public string[] GuildIds { get; set; }

        [JsonPropertyName("color")]
        public uint? Color { get; set; }
    }
}
