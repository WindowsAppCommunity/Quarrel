using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class GuildIntegrationUpdate
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
