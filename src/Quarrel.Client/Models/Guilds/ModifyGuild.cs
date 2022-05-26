// Quarrel © 2022

using Discord.API.Models.Json.Guilds;

namespace Quarrel.Client.Models.Guilds
{
    /// <summary>
    /// An object for modifying a guild.
    /// </summary>
    public class ModifyGuild
    {
        /// <summary>
        /// Gets or sets the guild's name.
        /// </summary>
        public string? Name { get; set; }

        internal JsonModifyGuild ToJsonModel()
        {
            return new JsonModifyGuild
            {
                Name = Name,
            };
        }
    }
}
