using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Game
{
    /// <summary>
    /// A service for games REST calls.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// A service for game REST calls.
        /// </summary>
        /// <returns>A list of games.</returns>
        [Get("/v6/applications")]
        Task<List<GameListItem>> GetGames();
    }
}
