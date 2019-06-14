using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.Shared.Services.Rest
{
    class DiscordClientInfo
    {
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }
    }
}
