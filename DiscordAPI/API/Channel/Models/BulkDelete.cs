using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Channel.Models
{
    public class BulkDelete
    {
        [JsonProperty("messages")]
        public IEnumerable<string> Messages { get; set; }
    }
}
