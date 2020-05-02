using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class GuildMemberRemove
    {
        [JsonProperty("guild_id")]
        public string guildId { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
