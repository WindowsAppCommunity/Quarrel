using Newtonsoft.Json;

namespace DiscordAPI.Voice.DownstreamEvents
{
    public class Hello
    {
        [JsonProperty("heartbeat_interval")]
        public int Heartbeatinterval { get; set; }
    }
}