using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class PruneStatus
    {
        [JsonProperty("pruned")]
        public int PrunedCount { get; set; }
    }
}
