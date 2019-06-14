using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Voice.UpstreamEvents
{
    public class Identify
    {
        [JsonProperty("server_id")]
        public string GuildId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("video")]
        public bool Video { get; set; }
    }
}
