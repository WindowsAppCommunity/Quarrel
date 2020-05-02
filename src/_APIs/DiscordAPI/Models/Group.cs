using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class Group
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
