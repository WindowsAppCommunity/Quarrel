using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice.UpstreamEvents
{
    public struct SelectProtocol
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("data")]
        public UdpProtocolInfo Data { get; set; }
    }

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
