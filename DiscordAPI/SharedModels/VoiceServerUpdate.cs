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
        public string Token { get; set; }
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("endpoint")]
        public string ServerId { get; set; }

        public string GetConnectionUrl()
        {
            return "wss://" + ServerId.Replace(":80", ""); //TODO: Remove after : not specifically ":80"
        }
    }
}
