// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// A request to bulk delete messages.
    /// </summary>
    public class BulkDelete
    {
        /// <summary>
        /// Gets or sts the list of messages to delete by ids.
        /// </summary>
        [JsonProperty("messages")]
        public IEnumerable<string> Messages { get; set; }
    }
}
