using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Activities
{
    public interface IActivitesService
    {
        [Get("/v6/activities")]
        Task<IEnumerable<ActivityData>> GetActivites();

        [Get("/v6/users/@me/feed/settings?include_autosubscribed_games=true")]
        Task<FeedSettings> GetFeedSettings();

        [Patch("/v6/users/@me/feed/settings")]
        Task<FeedSettings> PatchFeedSettings([Body] FeedPatch request);
        /// <summary>
        /// Get game news
        /// </summary>
        /// <param name="gameIds">An array of game IDs to get news for, seperated by "%2C"</param>
        /// <returns></returns>
        [Get("/v6/game-news?game_ids={gameIds}")]
        Task<List<GameNews>> GetGameNews([AliasAs("gameIds")] string gameIds);


    }
}
