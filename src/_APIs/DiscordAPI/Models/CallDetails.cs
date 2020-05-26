// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    /// <summary>
    /// The model for details of a call.
    /// </summary>
    public class CallDetails
    {
        /// <summary>
        /// Gets or sets the recipients on the call.
        /// </summary>
        [JsonProperty("recipients")]
        public IEnumerable<string> Recipients { get; set; }
    }
}
