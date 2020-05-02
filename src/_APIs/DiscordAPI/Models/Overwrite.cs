using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class Overwrite
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("allow")]
        public int Allow { get; set; }
        [JsonProperty("deny")]
        public int Deny { get; set; }
    }
}
