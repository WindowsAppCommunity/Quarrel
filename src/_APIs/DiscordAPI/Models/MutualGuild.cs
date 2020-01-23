using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class MutualGuild
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public bool HasNickname => !string.IsNullOrEmpty(Nick);
    }
}
