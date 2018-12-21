using Newtonsoft.Json;
using System;

namespace Discord_UWP.API.Misc.Models
{
    public class GifSearchResult
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("src")]
        public Uri Src { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }
}
