// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// A model for modifying a channel.
    /// </summary>
    public class ModifyChannel
    {
        /// <summary>
        /// Gets or sets the new name of the channel.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the new channel position.
        /// </summary>
        [JsonProperty("position")]
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the new channel topic.
        /// </summary>
        [JsonProperty("topic")]
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the new channel bitrate (for voice).
        /// </summary>
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        /// <summary>
        /// Gets or sets the new channel user limit (for voice).
        /// </summary>
        [JsonProperty("user_limit")]
        public int UserLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel should be nsfw.
        /// </summary>
        [JsonProperty("nsfw")]
        public bool NSFW { get; set; }

        /// <summary>
        /// Gets or sets the new permission overwrites for the chanel.
        /// </summary>
        [JsonProperty("permission_overwrites")]
        public IEnumerable<Overwrite> PermissionOverwrites { get; set; }

        /// <summary>
        /// Gets or sets the new category id.
        /// </summary>
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
    }
}
