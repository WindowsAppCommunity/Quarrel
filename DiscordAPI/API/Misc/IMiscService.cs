using Discord_UWP.API.Misc.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Discord_UWP.API.Misc
{
    public interface IMiscService
    {
        [Get("/integrations/giphy/search")]
        Task<IEnumerable<GifSearchResult>> SearchGiphy([AliasAs("q")] string searchQuery);
        [Get("/integrations/tenor/search")]
        Task<IEnumerable<GifSearchResult>> SearchTenor([AliasAs("q")] string searchQuery);
    }
}
