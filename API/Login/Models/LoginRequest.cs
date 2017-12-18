using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.Login.Models
{
    public struct LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
    public struct LoginMFARequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
    }
}
