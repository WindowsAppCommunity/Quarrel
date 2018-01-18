using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrenglyAPI
{
    public struct TranlateData
    {
        [JsonProperty("src")]
        public string InLan { get; set; }
        [JsonProperty("dest")]
        public string OutLan { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("premiumkey")]
        public string key { get; set; }
    }
}
