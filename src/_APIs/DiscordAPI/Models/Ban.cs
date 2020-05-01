using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class Ban
    {
        [JsonProperty("User")]
        public User User { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        public string GuildId { get; set; }
    }
}
