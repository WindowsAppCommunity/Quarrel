using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.API.Misc.Models;


namespace Quarrel.API.Misc
{
    public interface IMiscService
    {
        [Get("/integrations/giphy/search")]
        Task<IEnumerable<GifSearchResult>> SearchGiphy([AliasAs("q")] string searchQuery);
        [Get("/integrations/tenor/search")]
        Task<IEnumerable<GifSearchResult>> SearchTenor([AliasAs("q")] string searchQuery);
    }
}
