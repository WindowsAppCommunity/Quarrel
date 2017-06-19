using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct Friend
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("user")]
        public User user { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
