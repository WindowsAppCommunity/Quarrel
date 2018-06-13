using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.Guild.Models
{
    public class CreateGuildBan
    {
        [JsonProperty("delete-message-days")]
        public int DeleteMessageDays { get; set; }
    }
}
