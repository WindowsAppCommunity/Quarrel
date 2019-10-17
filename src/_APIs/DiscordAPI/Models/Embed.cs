using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Models
{
    public class Embed
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }
        [JsonProperty("image")]
        public EmbedImage Image { get; set; }
        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }
        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }
        [JsonProperty("provider")]
        public EmbedProvider Provider { get; set; }
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }
        [JsonProperty("fields")]
        public EmbedField[] Fields { get; set; }
    }

    public class EmbedThumbnail
    {
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
        [DefaultValue("")]
        [JsonProperty("proxy_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonIgnore]
        public string BindUrl => $"{ProxyUrl}?width=400&height={BindHeight}";

        [JsonIgnore]
        public int BindHeight=> (Height * 400) / Width;
    }

    public class EmbedImage
    {
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
        [DefaultValue("")]
        [JsonProperty("proxy_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }

    public class EmbedVideo
    {
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonIgnore]
        public double ActualHeight { get => Height != 0 ? Height : double.NaN; }

        [JsonIgnore]
        public double ActualWidth { get => Width != 0 ? Width : double.NaN; }
    }

    public class EmbedProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
    }

    public class EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [DefaultValue("")]
        [JsonProperty("icon_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string IconUrl { get; set; }
        [DefaultValue("")]
        [JsonProperty("proxy_icon_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ProxyIconUrl { get; set; }
    }

    public class EmbedAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [DefaultValue("")]
        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Url { get; set; }
        [DefaultValue("")]
        [JsonProperty("icon_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string IconUrl { get; set; }
        [DefaultValue("")]
        [JsonProperty("proxy_icon_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string ProxyIconUrl { get; set; }
    }

    public class EmbedField
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }
}
