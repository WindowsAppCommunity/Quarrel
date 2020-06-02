// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Login.Models
{
    /// <summary>
    /// An Sms request.
    /// </summary>
    public class SmsRequest
    {
        /// <summary>
        /// Gets or sets an sms request ticket.
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
    }
}
