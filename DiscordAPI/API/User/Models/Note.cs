using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.User.Models
{
    public class Note
    {
        [JsonProperty("note")]
        public string note { get; set; }
    }
}
