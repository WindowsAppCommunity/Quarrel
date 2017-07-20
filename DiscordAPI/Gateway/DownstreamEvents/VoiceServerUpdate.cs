using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct VoiceServerUpdate
    {
        [JsonProperty("token")]
        public string GuildId { get; set; }
        [JsonProperty("guild_id")]
        public string ChannelId { get; set; }
        [JsonProperty("endpoint")]
        public string SessionId { get; set; }
    }
}
