// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    /// <summary>
    /// A model representing a new recipient to a DM channel.
    /// </summary>
    public class ChannelRecipientUpdate
    {
        /// <summary>
        /// Gets or sets the user added.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the id of the channel the recipient was added to.
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
