using Newtonsoft.Json;

namespace DiscordStatusAPI.Models
{
    public class AffectedComponent
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("old_status")]
        public string OldStatus { get; set; }

        [JsonProperty("new_status")]
        public string NewStatus { get; set; }
    }
}
