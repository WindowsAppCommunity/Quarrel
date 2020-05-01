using Newtonsoft.Json;

namespace GiphyAPI.Models
{
    public struct Pagination
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
