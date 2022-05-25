// Quarrel © 2022

using Discord.API.Models.Enums.Settings;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Settings;
using System;
using System.Text.Json.Serialization;

namespace Quarrel.Client.Models.Settings
{
    /// <summary>
    /// An object for modifying user settings.
    /// </summary>
    public class ModifyUserSettings
    {
        /// <summary>
        /// Gets or sets the user's discord theme.
        /// </summary>
        [JsonPropertyName("theme")]
        public Theme? Theme { get; set; }

        /// <summary>
        /// Gets or sets the user's status.
        /// </summary>
        [JsonPropertyName("status")]
        public UserStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the list of guilds where users can DM from.
        /// </summary>
        [JsonPropertyName("restricted_guilds")]
        public ulong[]? RestrictedGuilds { get; set; }

        internal JsonModifyUserSettings ToJsonModel()
        {
            var json = new JsonModifyUserSettings
            {
                Status = Status?.GetStringValue(),
                Theme = Theme?.GetStringValue(),
                RestrictedGuilds = RestrictedGuilds,
            };

            return json;
        }
    }
}
