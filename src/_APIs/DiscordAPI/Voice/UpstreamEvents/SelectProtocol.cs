using Newtonsoft.Json;

namespace DiscordAPI.Voice.UpstreamEvents
{
    public class SelectProtocol
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }

    public class Codec
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("payloadType")]
        public int payloadType { get; set; }
        [JsonProperty("priority")]
        public int Priority { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("rtx_payload_type")]
        public int rtxPayloadType { get; set; }
    }

    public class UdpProtocolInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("codecs")]
        public Codec[] Codecs { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }
}
