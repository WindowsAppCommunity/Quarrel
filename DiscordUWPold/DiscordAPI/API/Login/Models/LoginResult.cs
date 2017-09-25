using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.Login.Models
{
    public struct LoginResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
