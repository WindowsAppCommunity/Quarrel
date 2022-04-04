﻿// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Messages.Embeds
{
    internal class JsonEmbedMedia
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}