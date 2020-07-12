// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Interfaces;
using Newtonsoft.Json;

namespace DiscordAPI.Models.Messages.Embeds
{
    public class Embed : IPreviewableImage
    {
        [JsonProperty("title")]
        public string Title { get; set; }
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

        [JsonIgnore]
        public string ImageUrl
        {
            get
            {
                if (Image != null)
                {
                    return Image.ProxyUrl;
                }
                else if (Thumbnail != null)
                {
                    return Thumbnail.ProxyUrl;
                }
                return null;
            }
        }

        [JsonIgnore]
        public double ImageHeight
        {
            get
            {
                if (Image != null)
                {
                    return Image.Height;
                }
                else if (Thumbnail != null)
                {
                    return Thumbnail.Height;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double ImageWidth
        {
            get
            {
                if (Image != null)
                {
                    return Image.Width;
                }
                else if (Thumbnail != null)
                {
                    return Thumbnail.Width;
                }
                return 0;
            }
        }

        [JsonIgnore]
        public string AnimatedImageUrl
        {
            get
            {
                if (Video != null)
                {
                    return Video.ProxyUrl;
                }
                return null;
            }
        }
    }

    public class EmbedThumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonIgnore]
        public string BindUrl => $"{ProxyUrl}?width=400&height={BindHeight}";

        [JsonIgnore]
        public int BindHeight => Width == 0 ? 400 : (Height * 400) / Width;
    }

    public class EmbedImage
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

    public class EmbedVideo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }

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
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }

    public class EmbedAuthor
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
