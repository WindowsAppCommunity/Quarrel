using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public class AllMetrics
    {
        [JsonProperty("period")]
        public Period Period { get; set; }

        [JsonProperty("metrics")]
        public MetricElement[] Metrics { get; set; }

        [JsonProperty("summary")]
        public Summary Summary { get; set; }
    }
}
