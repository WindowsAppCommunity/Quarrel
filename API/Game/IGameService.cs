using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Game
{
    public interface IGameService
    {
        [Get("/games")]
        Task<List<GameList>> GetGames();
    }
}
