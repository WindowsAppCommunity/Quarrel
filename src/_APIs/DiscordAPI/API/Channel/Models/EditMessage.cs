using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    public class EditMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
