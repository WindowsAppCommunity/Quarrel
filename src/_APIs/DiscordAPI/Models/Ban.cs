// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    /// <summary>
    /// The model for a user's ban.
    /// </summary>
    public class Ban
    {
        /// <summary>
        /// Gets or sets the user banned.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the reason the user was banned.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the id of the guild the user was banned from.
        /// </summary>
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
