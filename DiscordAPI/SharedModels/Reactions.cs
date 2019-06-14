using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Reactions
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("me")]
        public bool Me { get; set; }
        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }
    }
}
