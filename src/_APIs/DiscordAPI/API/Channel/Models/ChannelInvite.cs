// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// An invite to channel.
    /// </summary>
    public class ChannelInvite
    {
        /// <summary>
        /// Gets or sets the duration of invite in seconds before expiry.
        /// </summary>
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }

        /// <summary>
        /// Gets or sets the max number of uses.
        /// </summary>
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this invite only grants temporary membership.
        /// </summary>
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
    }
}
