// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    /// <summary>
    /// A model for an Emoji.
    /// </summary>
    public class Emoji
    {
        /// <summary>
        /// Gets or sets the id of the emoji.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the emoji.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Emoji requires colons.
        /// </summary>
        [JsonProperty("require_colons")]
        public bool RequireColons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the emoji is managed.
        /// </summary>
        [JsonProperty("managed")]
        public bool Managed { get; set; }

        [JsonProperty("animated")]
        public bool Animated { get; set; }

        [JsonIgnore]
        public bool IsServer { get => Id != null; }

        [JsonIgnore]
        public string DisplayUrl
        {
            get => "https://cdn.discordapp.com/emojis/" + Id
+ (Animated ? ".gif" : ".png");
        }
    }
}
