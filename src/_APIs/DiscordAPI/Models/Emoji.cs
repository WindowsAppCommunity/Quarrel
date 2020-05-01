using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class Emoji
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }

        [JsonProperty("require_colons")]
        public bool RequireColons { get; set; }

        [JsonProperty("managed")]
        public bool Managed { get; set; }

        [JsonProperty("animated")]
        public bool Animated { get; set; }

        [JsonIgnore]
        public bool IsServer { get => Id != null; }

        [JsonIgnore]
        public string DisplayUrl
        {
            get => "https://cdn.discordapp.com/emojis/" + Id
+ (Animated ? ".gif" : ".png");
        }
    }
}
