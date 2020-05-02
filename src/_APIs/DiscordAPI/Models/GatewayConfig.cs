using Newtonsoft.Json;

namespace DiscordAPI.Models
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
