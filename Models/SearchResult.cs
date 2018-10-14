using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
