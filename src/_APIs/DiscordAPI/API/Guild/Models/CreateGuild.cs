using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateGuild
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("icon")]
        public string Base64Icon { get; set; }
    }
}
