// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Login.Models
{
    /// <summary>
    /// An sms request result.
    /// </summary>
    public class SmsResult
    {
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
    }
}
