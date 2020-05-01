using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    public class EditChannel
    {
        [JsonProperty("allow")]
        public int Allow { get; set; }
        [JsonProperty("deny")]
        public int Deny { get; set; }
    }
}
