// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages.Embeds
{
    internal class JsonEmbedFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
        
        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}
