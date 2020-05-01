using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public partial class MetricElement
    {
        [JsonProperty("metric")]
        public Metric Metric { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }
}
