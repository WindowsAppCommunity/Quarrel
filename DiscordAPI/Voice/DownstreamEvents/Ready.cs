using DiscordAPI.SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Voice.DownstreamEvents
{
    public class Ready
    {
        [JsonProperty("ssrc")]
        public uint SSRC { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("modes")]
        public IEnumerable<string> Modes { get; set; }
        [JsonProperty("heartbeat_interval")]
        public int Heartbeatinterval { get; set; }
    }
}