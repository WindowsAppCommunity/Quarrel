// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Login.Models
{
    /// <summary>
    /// A login request with MFA.
    /// </summary>
    public class LoginMFARequest
    {
        /// <summary>
        /// MFA code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// MRA ticket.
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
    }
}
