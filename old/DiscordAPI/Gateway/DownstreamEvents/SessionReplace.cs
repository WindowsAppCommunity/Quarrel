using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway.DownstreamEvents
{
    public class SessionReplace
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("game")]
        public object Game { get; set; }
        [JsonProperty("client_info")]
        public ClientInfo clientInfo { get; set; }

        public class ClientInfo
        {
            [JsonProperty("version")]
            public int Version { get; set; }
            [JsonProperty("os")]
            public string OS { get; set; }
            [JsonProperty("client")]
            public string Client { get; set; }
        }
    }
}
