using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateGuildChannel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public class CreateVoiceChannel : CreateGuildChannel
    {
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        [JsonProperty("user_limit")]
        public int UserLimit { get; set; }
    }
}
