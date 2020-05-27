// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// Channel permissions.
    /// </summary>
    public class EditChannelPermissions
    {
        /// <summary>
        /// Gets or sets the new allowed permissions.
        /// </summary>
        [JsonProperty("allow")]
        public int Allow { get; set; }

        /// <summary>
        /// Gets or sets the new denied permissions.
        /// </summary>
        [JsonProperty("deny")]
        public int Deny { get; set; }
    }
}
