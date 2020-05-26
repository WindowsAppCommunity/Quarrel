// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models.Channels
{
    /// <summary>
    /// A model for a Guild channel.
    /// </summary>
    public class GuildChannel : Channel
    {
        /// <summary>
        /// Gets or sets the id of the channel's guild.
        /// </summary>
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        /// <summary>
        /// Gets or sets the id of channel's parent category.
        /// </summary>
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the position of the channel in the guild.
        /// </summary>
        [JsonProperty("position")]
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel is private.
        /// </summary>
        [JsonProperty("is_private")]
        public bool Private { get; set; }

        /// <summary>
        /// Gets or sets the permission overwrites in the channel.
        /// </summary>
        [JsonProperty("permission_overwrites")]
        public IEnumerable<Overwrite> PermissionOverwrites { get; set; }

        /// <summary>
        /// Gets or sets the topic of the channel.
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the bitrate of the channel.
        /// </summary>
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        /// <summary>
        /// Gets or sets the user limit of the channel.
        /// </summary>
        [JsonProperty("user_limit")]
        public string UserLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel is NSFW.
        /// </summary>
        [JsonProperty("nsfw")]
        public bool NSFW { get; set; }
    }
}
