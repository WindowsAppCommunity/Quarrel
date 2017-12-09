using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct Attachment
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
    }
}
