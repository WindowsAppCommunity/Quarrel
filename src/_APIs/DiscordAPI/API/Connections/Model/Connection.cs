// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Connections
{
    /// <summary>
    /// An Oauth connection.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// The connection's url.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
