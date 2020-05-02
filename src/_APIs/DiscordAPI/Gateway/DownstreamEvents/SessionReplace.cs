using DiscordAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class SessionReplace
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
        [JsonProperty("client_info")]
        public ClientInfo clientInfo { get; set; }
        [JsonProperty("activities")]
        public IEnumerable<object> Activities;

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
