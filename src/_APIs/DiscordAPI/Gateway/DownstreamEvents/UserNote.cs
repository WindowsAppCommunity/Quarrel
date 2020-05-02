using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class UserNote
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
