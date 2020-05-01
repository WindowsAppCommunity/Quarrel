using DiscordAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    public class GuildEmojiUpdate
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
        [JsonProperty("emojis")]
        public IEnumerable<Emoji> Unavailable { get; set; }
    }
}
