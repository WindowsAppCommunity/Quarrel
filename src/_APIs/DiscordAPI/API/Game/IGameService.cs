using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Game
{
    public interface IGameService
    {
        [Get("/v6/applications")]
        Task<List<GameListItem>> GetGames();
    }
}
