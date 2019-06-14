using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild.Models
{
    public class CreateGuildBan
    {
        [JsonProperty("delete-message-days")]
        public int DeleteMessageDays { get; set; }
    }
}
