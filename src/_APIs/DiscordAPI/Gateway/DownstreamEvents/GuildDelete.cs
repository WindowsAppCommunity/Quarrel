using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class GuildDelete
    {
        [JsonProperty("id")]
        public string GuildId { get; set; }
        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }
    }
}
