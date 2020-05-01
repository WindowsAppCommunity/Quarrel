using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateIntegration
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
