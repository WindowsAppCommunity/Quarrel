using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Ban
    {
        [JsonProperty("User")]
        public User User { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        public string GuildId { get; set; }
    }
}
