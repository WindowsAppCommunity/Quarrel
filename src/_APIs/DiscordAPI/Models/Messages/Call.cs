// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models.Messages
{
    /// <summary>
    /// The model for a call.
    /// </summary>
    public class Call
    {
        /// <summary>
        /// Gets or sets the participents in the call.
        /// </summary>
        [JsonProperty("participants")]
        public IEnumerable<string> Participants { get; set; }

        /// <summary>
        /// Gets or sets the timestamp the call ended.
        /// </summary>
        [JsonProperty("ended_timestamp")]
        public string EndedTimestamp { get; set; }
    }
}
