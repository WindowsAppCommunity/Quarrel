// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Activites
{
    /// <summary>
    /// A model for info on the thumbnail of game news.
    /// </summary>
    public partial class GameNewsMedia
    {
        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        [JsonProperty("height")]
        public long Height { get; set; }

        /// <summary>
        /// Gets or sets the proxied url image.
        /// </summary>
        [JsonProperty("proxy_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Gets or sets the image's raw url.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        [JsonProperty("width")]
        public long Width { get; set; }
    }
}
