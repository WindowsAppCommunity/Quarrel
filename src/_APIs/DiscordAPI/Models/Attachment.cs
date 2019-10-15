using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Models
{
    public class Attachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("size")]
        public ulong Size { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonIgnore]
        public double ActualHeight => Height.HasValue && Width.HasValue ? (double)Height.Value/Width.Value * Math.Min(Width.Value, 400) : double.NaN;

        [JsonIgnore]
        public double ActualWidth => Width.HasValue ? Math.Min(Width.Value, 400) : double.NaN;
    }
}
