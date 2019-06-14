using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild.Models
{
    public class ModifyGuild
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
        [JsonProperty("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }
        [JsonProperty("afk_channel_id")]
        public string AfkChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AfkTimeout { get; set; }
      //  [JsonProperty("owner_id")]
      //  public string OwnerId { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
    }

    public class ModifyGuildIcon : ModifyGuild
    {
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
