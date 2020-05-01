using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.User.Models
{
    public class CreateDM
    {
        [JsonProperty("recipients")]
        public IEnumerable<string> Recipients { get; set; }
    }
}
