using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Channel.Models
{
    public class EditMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
