using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway.UpstreamEvents
{
    public class GatewayResume
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public int LastSequenceNumberReceived { get; set; }
    }
}
