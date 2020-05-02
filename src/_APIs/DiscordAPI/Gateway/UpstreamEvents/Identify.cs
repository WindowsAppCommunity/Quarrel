using Newtonsoft.Json;

namespace DiscordAPI.Gateway.UpstreamEvents
{
    public class Identify
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
        [JsonProperty("compress")]
        public bool Compress => false;
        [JsonProperty("large_threshold")]
        public int LargeThreshold { get; set; }
    }

    public class Properties
    {
        [JsonProperty("$os")]
        public string OS { get; set; }
        [JsonProperty("$browser")]
        public string Browser { get; set; }
        [JsonProperty("$device")]
        public string Device { get; set; }
        [JsonProperty("$referrer")]
        public string Referrer { get; set; }
        [JsonProperty("$referring_domain")]
        public string ReferringDomain { get; set; }
    }
}
