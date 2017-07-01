using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct MutualGuild
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
