using Newtonsoft.Json;

namespace Discord_UWP.Voice
{
    public struct UdpProtocolInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}

