// Adam Dernis © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

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
