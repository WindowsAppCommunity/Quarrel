using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild.Models
{
    public class ModifyGuildChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
    }
}
