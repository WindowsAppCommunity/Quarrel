using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class ConnectedAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        public string ImagePath { get; set; }
    }
}
