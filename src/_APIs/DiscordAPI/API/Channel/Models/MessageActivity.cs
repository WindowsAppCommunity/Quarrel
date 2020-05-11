// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// Activity bound to message.
    /// </summary>
    public class MessageActivity
    {
        /// <summary>
        /// Gets or sets the activity session id.
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the activity type.
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the party id.
        /// </summary>
        [JsonProperty("party_id")]
        public string PartyId { get; set; }
    }
}
