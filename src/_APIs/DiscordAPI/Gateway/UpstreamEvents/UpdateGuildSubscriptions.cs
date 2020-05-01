using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.UpstreamEvents
{
    public class UpdateGuildSubscriptions
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("typing")]
        public bool? Typing { get; set; }

        [JsonProperty("activities")]
        public bool? Activities { get; set; }

        [JsonProperty("channels")]
        public IReadOnlyDictionary<string, IEnumerable<int[]>> Channels { get; set; }

        [JsonProperty("members")]
        public IEnumerable<string> Members { get; set; }
    }

}
