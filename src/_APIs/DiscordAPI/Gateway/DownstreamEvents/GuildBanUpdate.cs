using DiscordAPI.Models;
using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class GuildBanUpdate
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }

    }
}
