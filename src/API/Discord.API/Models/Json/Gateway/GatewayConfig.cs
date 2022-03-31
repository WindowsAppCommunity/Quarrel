// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Gateway
{
    internal class GatewayConfig
    {
        [JsonPropertyName("url")]
        public string BaseUrl { get; set; }

        public string GetFullGatewayUrl(string encodingType, string version, string append)
        {
            return $"{BaseUrl}/?encoding={encodingType}&v={version}{append}";
        }
    }
}
