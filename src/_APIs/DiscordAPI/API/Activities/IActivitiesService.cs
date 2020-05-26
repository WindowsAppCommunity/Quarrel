// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Activites;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Activities
{
    /// <summary>
    /// An interface representing access to activites related endoints.
    /// </summary>
    public interface IActivitiesService
    {
        /// <summary>
        /// Gets a list of activities from Discord.
        /// </summary>
        /// <returns>The list of Activities.</returns>
        [Get("/v6/activities")]
        Task<IEnumerable<ActivityData>> GetActivities();

        /// <summary>
        /// Gets the feed settings for the current user.
        /// </summary>
        /// <returns>The feed settings.</returns>
        [Get("/v6/users/@me/feed/settings?include_autosubscribed_games=true")]
        Task<FeedSettings> GetFeedSettings();

        /// <summary>
        /// Adjusted the feed settings for the current user.
        /// </summary>
        /// <param name="request">The requested adjustments to feed settings.</param>
        /// <returns>The new feed settings.</returns>
        [Patch("/v6/users/@me/feed/settings")]
        Task<FeedSettings> PatchFeedSettings([Body] FeedPatch request);

        /// <summary>
        /// Get game news.
        /// </summary>
        /// <param name="gameIds">An array of game IDs to get news for, seperated by "%2C".</param>
        /// <returns>A list of news on the requested games.</returns>
        [Get("/v6/game-news?game_ids={gameIds}")]
        Task<List<GameNews>> GetGameNews([AliasAs("gameIds")] string gameIds);
    }
}
