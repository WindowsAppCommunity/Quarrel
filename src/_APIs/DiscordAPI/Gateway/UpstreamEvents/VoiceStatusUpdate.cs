using Newtonsoft.Json;

namespace DiscordAPI.Gateway.UpstreamEvents
{

    public class VoiceStatusUpdate
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("self_mute")]
        public bool Mute { get; set; }
        [JsonProperty("self_deaf")]
        public bool Deaf { get; set; }
    }
}
