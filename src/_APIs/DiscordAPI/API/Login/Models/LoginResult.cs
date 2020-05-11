// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordAPI.API.Login.Models
{
    /// <summary>
    /// A login result.
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// Gets or sets a login result token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not MFA is required.
        /// </summary>
        [JsonProperty("mfa")]
        public bool MFA { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not sms is supported.
        /// </summary>
        [JsonProperty("sms")]
        public bool SmsSupported { get; set; }

        /// <summary>
        /// A login result ticket.
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        /// <summary>
        /// Gets or sets a captcha key.
        /// </summary>
        [JsonProperty("captcha_key")]
        public List<string> CaptchaKey { get; set; }

        /// <summary>
        /// Gets or sets login emails.
        /// </summary>
        [JsonProperty("email")]
        public List<string> Email { get; set; }

        /// <summary>
        /// Gets or sets login passwords.
        /// </summary>
        [JsonProperty("password")]
        public List<string> Password { get; set; }
    }
}
