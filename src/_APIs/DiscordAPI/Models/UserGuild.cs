using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class UserGuild
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("owner")]
        public bool Owner { get; set; }
        [JsonProperty("permissions")]
        public int Permissions { get; set; }
    }
}
