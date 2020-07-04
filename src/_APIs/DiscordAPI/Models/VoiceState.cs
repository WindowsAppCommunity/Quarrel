using Newtonsoft.Json;

namespace DiscordAPI.Models
{
    public class VoiceState
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("member")]
        public GuildMember Member { get; set; }
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

        [JsonIgnore]
        public bool IsMuted => SelfMute || ServerMute;

        [JsonIgnore]
        public bool IsDeafened => SelfDeaf || ServerDeaf;
    }
}
