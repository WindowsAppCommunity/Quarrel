using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.User.Models
{
    public struct Note
    {
        [JsonProperty("note")]
        public string note { get; set; }
    }
}
