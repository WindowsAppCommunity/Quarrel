using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.API.Game
{
    public interface IGameService
    {
        [Get("/v6/applications")]
        Task<List<GameListItem>> GetGames();
    }
}
