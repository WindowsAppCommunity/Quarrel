using Newtonsoft.Json;

namespace Discord_UWP.Voice.UpstreamEvents
{
    public struct SelectProtocolParams
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }
}
