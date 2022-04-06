// Adam Dernis © 2022

using Discord.API.Models.Json.Settings;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Settings
{
    /// <summary>
    /// A guild folder managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildFolder
    {
        internal GuildFolder(JsonGuildFolder jsonGuildFolder)
        {
            Id = jsonGuildFolder.Id;
            Name = jsonGuildFolder.Name;
            GuildIds = jsonGuildFolder.GuildIds;
            Color = jsonGuildFolder.Color;
        }

        /// <summary>
        /// Gets the id of the guild folder.
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; private set; }

        /// <summary>
        /// Gets the name of the guild folder.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; private set; }

        /// <summary>
        /// Gets the color of the guild folder.
        /// </summary>
        [JsonPropertyName("color")]
        public uint? Color { get; private set; }

        /// <summary>
        /// Gets the list of child guilds in the folder.
        /// </summary>
        [JsonPropertyName("guild_ids")]
        public ulong[] GuildIds { get; private set; }
    }
}
