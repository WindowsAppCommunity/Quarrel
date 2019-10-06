using Newtonsoft.Json;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class InvalidSession
    {
        [JsonProperty("d")]
        public bool ConnectedState { get; set; } = false;
    }
}