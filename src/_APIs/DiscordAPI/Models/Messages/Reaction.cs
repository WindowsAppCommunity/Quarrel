// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Messages
{
    /// <summary>
    /// A model for a reaction.
    /// </summary>
    public class Reaction
    {
        /// <summary>
        /// Gets or sets the amount of uses of the reaction.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current has used the reaction.
        /// </summary>
        [JsonProperty("me")]
        public bool Me { get; set; }

        /// <summary>
        /// Gets or sets the emoji reaction.
        /// </summary>
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        /// <summary>
        /// Gets or sets id of the message the reaction is on.
        /// </summary>
        [JsonIgnore]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the channel of the message.
        /// </summary>
        [JsonIgnore]
        public string ChannelId { get; set; }
    }
}
