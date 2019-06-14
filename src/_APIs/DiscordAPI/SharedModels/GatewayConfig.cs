using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class GatewayConfig
    {
        [JsonProperty("url")]
        public string BaseUrl { get; set; }

        public string GetFullGatewayUrl(string encodingType, string version, string append)
        {
            return $"{BaseUrl}/?encoding={encodingType}&v={version}{append}";
        }
    }
}
