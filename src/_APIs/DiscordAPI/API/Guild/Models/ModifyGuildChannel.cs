using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class ModifyGuildChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
    }
}
