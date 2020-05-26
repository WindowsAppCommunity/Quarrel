// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Game.Models
{
    /// <summary>
    /// Represents and executable file.
    /// </summary>
    public class Executable
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the operating system.
        /// </summary>
        [JsonProperty("os")]
        public string Os { get; set; }
    }
}
