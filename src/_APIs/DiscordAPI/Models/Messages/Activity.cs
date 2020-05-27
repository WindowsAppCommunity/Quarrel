// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Messages
{
    /// <summary>
    /// A Model for an activity embeded in a message.
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Gets or sets the id of the party for the activity.
        /// </summary>
        [JsonProperty("party_id")]
        public string PartyId { get; set; }

        /// <summary>
        /// Gets or sets the type of the activity.
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
