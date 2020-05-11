// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Game.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Game
{
    /// <summary>
    /// A game list item.
    /// </summary>
    public class GameListItem
    {
        /// <summary>
        /// Gets or sets the game's list of executable files.
        /// </summary>
        [JsonProperty("executables")]
        public List<Executable> Executables { get; set; }

        /// <summary>
        /// Gets or sets the game's icon hash.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the game's id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the game's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the game's summary.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the game's splash image hash.
        /// </summary>
        [JsonProperty("splash")]
        public string Splash { get; set; }

        /// <summary>
        /// Gets or sets the game's alias names.
        /// </summary>
        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; }

        /// <summary>
        /// Gets or sets the game's distributors.
        /// </summary>
        [JsonProperty("distributor_applications")]
        public List<DistributorGame> DistributorGames { get; set; }

        /// <summary>
        /// Gets or sets the game's developers.
        /// </summary>
        [JsonProperty("developers")]
        public List<string> Developers { get; set; }

        /// <summary>
        /// Gets or sets the game's publishers.
        /// </summary>
        [JsonProperty("publishers")]
        public List<string> Publishers { get; set; }

        /// <summary>
        /// Gets or sets the game's application id.
        /// </summary>
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the game's youtube trailer video id.
        /// </summary>
        [JsonProperty("youtube_trailer_video_id")]
        public string YoutubeTrailerId { get; set; }
    }
}
