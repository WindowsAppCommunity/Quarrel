using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class CallDetails
    {
        [JsonProperty("recipients")]
        public IEnumerable<string> Recipients { get; set; }
    }
}
