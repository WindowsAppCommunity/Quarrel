using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.User.Models
{
    public class SendFriendRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("discriminator")]
        public int Discriminator { get; set; }
    }
}
