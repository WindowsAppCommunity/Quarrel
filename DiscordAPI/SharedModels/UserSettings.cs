using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct UserSettings
    {
        [JsonProperty("theme")]
        public string Theme { get; set; }
        [JsonProperty("guild_positions")]
        public IEnumerable<string> GuildOrder { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
    }
}
