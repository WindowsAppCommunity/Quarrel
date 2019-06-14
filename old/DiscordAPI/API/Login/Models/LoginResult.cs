using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Login.Models
{
    public class LoginResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("mfa")]
        public bool MFA { get; set; }

        [JsonProperty("sms")]
        public bool SmsSupported { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("captcha_key")]
        public List<string> CaptchaKey { get; set; }
        [JsonProperty("email")]
        public List<string> Email { get; set; }
        [JsonProperty("password")]
        public List<string> Password { get; set; }

        public Exception exception { get; set; }
    }
    public class SendSmsResult
    {
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
    }
}
