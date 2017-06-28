using Discord_UWP.SharedModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public struct Ready
    {
        [JsonProperty("v")]
        public int GatewayVersion { get; set; }
        [JsonProperty("user_settings")]
        public UserSettings Settings { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("private_channels")]
        public IEnumerable<DirectMessageChannel> PrivateChannels { get; set; }
        [JsonProperty("guilds")]
        public IEnumerable<Guild> Guilds { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("presences")]
        public IEnumerable<Presence> Presences { get; set; }
        [JsonProperty("relationships")]
        public IEnumerable<Friend> Friends { get; set; }
        [JsonProperty("_trace")]
        public IEnumerable<string> Trace { get; set; }
    }
}
