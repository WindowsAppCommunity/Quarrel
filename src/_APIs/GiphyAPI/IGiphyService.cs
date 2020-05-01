using GiphyAPI.Models;
using Refit;
using System.Threading.Tasks;

namespace GiphyAPI
{
    public interface IGiphyService
    {
        [Get("/v1/gifs/search?api_key=erGe4TVabEDlDPOkHFc389gQPvx4ze9Z&q={query}&limit={limit}&offset={offset}&fmt=json")]
        Task<SearchResult> Search([AliasAs("query")] string query, [AliasAs("limit")]int limit = 20, [AliasAs("offset")] int offset = 0);

        [Get("/v1/gifs/trending?api_key=erGe4TVabEDlDPOkHFc389gQPvx4ze9Z&limit={limit}&offset={offset}")]
        Task<SearchResult> Trending([AliasAs("limit")] int limit = 20, [AliasAs("offset")] int offset = 0);
    }
}
