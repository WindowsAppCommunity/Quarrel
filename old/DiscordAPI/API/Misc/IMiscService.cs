using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.API.Misc.Models;


namespace DiscordAPI.API.Misc
{
    public interface IMiscService
    {
        [Get("/integrations/giphy/search")]
        Task<IEnumerable<GifSearchResult>> SearchGiphy([AliasAs("q")] string searchQuery);
        [Get("/integrations/tenor/search")]
        Task<IEnumerable<GifSearchResult>> SearchTenor([AliasAs("q")] string searchQuery);
    }
}
