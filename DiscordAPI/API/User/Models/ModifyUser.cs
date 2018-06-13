using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.User.Models
{
    public class ModifyUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
    }
    public class ModifyUserAndAvatar : ModifyUser
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
