using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateGuildChannel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public int UserLimit { get; set; }
    }
}
