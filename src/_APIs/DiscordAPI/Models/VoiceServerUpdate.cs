using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class VoiceServerUpdate
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        public string GetConnectionUrl(string version)
        {
            return "wss://" + Endpoint.Substring(0, Endpoint.LastIndexOf(':')) + "?v=" + version;
        }
    }
}
