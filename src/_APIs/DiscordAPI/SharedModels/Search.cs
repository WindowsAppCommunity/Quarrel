using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class Search
    {
        [JsonProperty("guild_id")]
        public List<string> guild_id { get; set; }

        [JsonProperty("query")]
        public string query { get; set; }

        [JsonProperty("limit")]
        public int limit { get; set; }
    }
}
