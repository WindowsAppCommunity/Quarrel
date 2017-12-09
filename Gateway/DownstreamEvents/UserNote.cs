using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord_UWP.Gateway.DownstreamEvents
{
    public struct UserNote
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
