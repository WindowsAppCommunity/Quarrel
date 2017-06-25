using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct Role
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("permissions")]
        public string Permissions { get; set; }
        [JsonProperty("managed")]
        public bool Managed { get; set; }
        [JsonProperty("mentionable")]
        public bool Mentionable { get; set; }

        public int MemberCount { get; set; }
    }
}
