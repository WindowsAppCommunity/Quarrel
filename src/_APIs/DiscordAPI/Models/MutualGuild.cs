using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class MutualGuild
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public bool NickVisibility => !string.IsNullOrEmpty(Nick);
    }
}
