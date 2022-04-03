// Adam Dernis © 2022

using Discord.API.Models.Json.Settings;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Settings
{
    public class GuildFolder
    {
        internal GuildFolder(JsonGuildFolder jsonGuildFolder)
        {
            Id = jsonGuildFolder.Id;
            Name = jsonGuildFolder.Name;
            GuildIds = jsonGuildFolder.GuildIds;
            Color = jsonGuildFolder.Color;
        }

        [JsonPropertyName("id")]
        public ulong? Id { get; private set; }

        [JsonPropertyName("name")]
        public string? Name { get; private set; }

        [JsonPropertyName("color")]
        public uint? Color { get; private set; }

        [JsonPropertyName("guild_ids")]
        public ulong[] GuildIds { get; private set; }
    }
}
