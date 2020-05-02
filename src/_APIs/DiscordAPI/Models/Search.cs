using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
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
