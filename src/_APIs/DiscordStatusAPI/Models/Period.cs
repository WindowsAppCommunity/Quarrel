using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public partial class Period
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("interval")]
        public long Interval { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }
    }
}
