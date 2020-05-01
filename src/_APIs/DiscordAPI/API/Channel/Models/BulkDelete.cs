using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Channel.Models
{
    public class BulkDelete
    {
        [JsonProperty("messages")]
        public IEnumerable<string> Messages { get; set; }
    }
}
