using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct GatewayConfig
    {
        [JsonProperty("url")]
        public string BaseUrl { get; set; }

        public string GetFullGatewayUrl(string encodingType, string version)
        {
            return $"{BaseUrl}/?encoding={encodingType}&v={version}";
        }
    }
}
