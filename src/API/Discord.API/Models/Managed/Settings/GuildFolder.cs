// Quarrel © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Guilds;
using Discord.API.Models.Json.Settings;
using System;

namespace Discord.API.Models.Settings
{
    /// <summary>
    /// A guild folder managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildFolder : DiscordItem
    {
        internal GuildFolder(JsonGuildFolder jsonGuildFolder, DiscordClient context) :
            base(context)
        {
            Id = jsonGuildFolder.Id;
            Name = jsonGuildFolder.Name;
            Color = jsonGuildFolder.Color;
            GuildIds = jsonGuildFolder.GuildIds;
        }

        /// <summary>
        /// Gets the id of the guild folder.
        /// </summary>
        public long? Id { get; private set; }

        /// <summary>
        /// Gets the name of the guild folder.
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// Gets the color of the guild folder.
        /// </summary>
        public uint? Color { get; private set; }

        /// <summary>
        /// Gets the list of child guilds in the folder.
        /// </summary>
        public ulong[] GuildIds { get; private set; }

        public Guild[] GetGuilds()
        {
            Guild[] guilds = new Guild[GuildIds.Length];
            int i = 0;
            foreach (var guildId in GuildIds)
            {
                Guild? guild = Context.GetGuildInternal(guildId)!;
                if (guild is Guild guildChannel)
                {
                    guilds[i] = guildChannel;
                    i++;
                }
            }

            Array.Resize(ref guilds, i);

            return guilds;
        }
    }
}
