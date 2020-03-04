// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace Quarrel.Shared.Services.Rest
{
    /// <summary>
    /// Client info for HttpClient.
    /// </summary>
    public class DiscordClientInfo
    {
        /// <summary>
        /// Gets or sets HttpClient clientId.
        /// </summary>
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets HttpClient UserAgent.
        /// </summary>
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }
    }
}
