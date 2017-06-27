﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.SharedModels
{
    public struct Embed
    {
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
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

    public struct EmbedThumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }

    public struct EmbedImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }

    public struct EmbedVideo
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
    }

    public struct EmbedProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public struct EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }

    public struct EmbedAuthor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }

    public struct EmbedField
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }
}
