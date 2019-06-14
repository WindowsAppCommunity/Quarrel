using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild.Models
{
    public class ModifyIntegration
    {
        [JsonProperty("expire_behavior")]
        public int ExpireBehaviour { get; set; }
        [JsonProperty("expire_grace_period")]
        public int ExpireGracePeriod { get; set; }
        [JsonProperty("enable_emoticons")]
        public bool EnableEmoticions { get; set; }
    }
}
