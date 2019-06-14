using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Channel.Models
{
    public class EditChannel
    {
        [JsonProperty("allow")]
        public int Allow { get; set; }
        [JsonProperty("deny")]
        public int Deny { get; set; }
    }
}
