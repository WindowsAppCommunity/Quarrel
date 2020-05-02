using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public partial class Summary
    {
        [JsonProperty("sum")]
        public double Sum { get; set; }

        [JsonProperty("mean")]
        public double Mean { get; set; }
    }
}