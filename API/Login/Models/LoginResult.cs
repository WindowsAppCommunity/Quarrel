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

        [JsonProperty("mfa")]
        public bool MFA { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("captcha_key")]
        public List<string> CaptchaKey { get; set; }

        public Exception exception { get; set; }
    }
}
