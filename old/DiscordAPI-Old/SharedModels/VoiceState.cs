using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class VoiceState
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("deaf")]
        public bool ServerDeaf { get; set; }
        [JsonProperty("mute")]
        public bool ServerMute { get; set; }
        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }
        [JsonProperty("suppress")]
        public bool Suppress { get; set; }
    }
}
