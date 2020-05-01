using Newtonsoft.Json;

namespace DiscordAPI.API.Connections
{
    public class Connection
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
