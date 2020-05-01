using Newtonsoft.Json;
using System.Collections.Generic;

namespace GiphyAPI.Models
{
    public struct SearchResult
    {
        [JsonProperty("data")]
        public IEnumerable<Gif> Gif { get; set; }

        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
