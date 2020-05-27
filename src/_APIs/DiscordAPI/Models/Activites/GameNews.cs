// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models.Activites
{
    /// <summary>
    /// The model for news on a game.
    /// </summary>
    public class GameNews
    {
        /// <summary>
        /// Gets or sets the description of the news.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the title of the news.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the url for the news.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the news.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a video for the news.
        /// </summary>
        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public GameNewsMedia Video { get; set; }

        /// <summary>
        /// Gets or sets the id of the game the news is for.
        /// </summary>
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        /// <summary>
        /// Gets or sets the type of the news.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail for the news.
        /// </summary>
        [JsonProperty("thumbnail")]
        public GameNewsMedia Thumbnail { get; set; }

        /// <summary>
        /// Gets or sets the title of the game the news is for.
        /// </summary>
        public string GameTitle { get; set; }
    }
}
