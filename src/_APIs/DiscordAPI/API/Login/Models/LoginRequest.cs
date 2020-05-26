// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Login.Models
{
    /// <summary>
    /// A login request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the login email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the login password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the captcha keys.
        /// </summary>
        [JsonProperty("captcha_key")]
        public string[] CaptchaKey { get; set; }
    }
}
