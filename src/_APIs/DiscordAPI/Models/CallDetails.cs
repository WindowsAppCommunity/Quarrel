using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.Models
{
    public class CallDetails
    {
        [JsonProperty("recipients")]
        public IEnumerable<string> Recipients { get; set; }
    }
}
