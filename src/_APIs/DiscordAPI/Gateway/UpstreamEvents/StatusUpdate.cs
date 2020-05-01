using DiscordAPI.Models;
using Newtonsoft.Json;

namespace DiscordAPI.Gateway.UpstreamEvents
{

    public struct StatusUpdate
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("since")]
        public long? IdleSince { get; set; }
        [JsonProperty("afk")]
        public bool IsAFK { get; set; }
        [JsonProperty("game")]
        public GameBase Game { get; set; }
    }
}
