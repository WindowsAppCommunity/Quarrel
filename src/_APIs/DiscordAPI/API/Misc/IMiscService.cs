using DiscordAPI.API.Misc.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;


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
