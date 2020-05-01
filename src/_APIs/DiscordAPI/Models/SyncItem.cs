using Newtonsoft.Json;

namespace DiscordAPI.Models
{

    public class SyncItem
    {
        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("member")]
        public GuildMemberPresence Member { get; set; }

    }
}
