// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Activites
{
    /// <summary>
    /// A model for a user's feed settings.
    /// </summary>
    public class FeedSettings
    {
        /// <summary>
        /// Gets or sets the user's subscribed games.
        /// </summary>
        [JsonProperty("subscribed_games")]
        public string[] SubscribedGames { get; set; }

        /// <summary>
        /// Gets or sets the user's automatically subscribed games.
        /// </summary>
        [JsonProperty("autosubscribed_games")]
        public string[] AutoSubscribedGames { get; set; }

        /// <summary>
        /// Gets or sets the user's unsubscribed games.
        /// </summary>
        [JsonProperty("unsubscribed_games")]
        public string[] UnsubscribedGames { get; set; }

        /// <summary>
        /// Gets or sets the user's subscribed users.
        /// </summary>
        [JsonProperty("subscribed_users")]
        public string[] SubscribedUsers { get; set; }

        /// <summary>
        /// Gets or sets user's unsuscribed users.
        /// </summary>
        [JsonProperty("unsubscribed_users")]
        public string[] UnsubscribedUsers { get; set; }
    }
}
