using Newtonsoft.Json;

namespace DiscordAPI.API.Misc.Models
{
    public class GifSearchResult
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        public long AdjustedWidth { get; set; }
    }
}
