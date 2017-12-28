using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GiphyAPI.Models;

namespace GiphyAPI
{
    public interface IGiphyService
    {
        [Get("/v1/gifs/search?api_key={"+GiphyAPI.GiphyKey+"}&q={query}&limit={limit}&offset={offset}&fmt=json")]
        Task<SearchResult> Search( [AliasAs("query")] string query, [AliasAs("limit")]int limit = 20, [AliasAs("offset")] int offset = 0);

        [Get("/v1/gifs/trending&api_key={" + GiphyAPI.GiphyKey + "}&limit={limit}&offset={offset}")]
        Task<SearchResult> Trending([AliasAs("limit")] int limit = 20, [AliasAs("offset")] int offset = 0);
    }
}
