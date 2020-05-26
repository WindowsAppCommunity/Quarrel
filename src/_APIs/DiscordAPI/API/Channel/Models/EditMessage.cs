// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// An edit message model.
    /// </summary>
    public class EditMessage
    {
        /// <summary>
        /// Gets or sets the new content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
