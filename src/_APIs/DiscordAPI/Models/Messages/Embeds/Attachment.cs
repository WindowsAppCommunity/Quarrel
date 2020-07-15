// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Interfaces;
using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models.Messages.Embeds
{
    /// <summary>
    /// A model for a message attachment.
    /// </summary>
    public class Attachment : IPreviewableImage
    {
        /// <summary>
        /// Gets or sets the Id of the attachment.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the filename in the attachment.
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the size of file in the attachment.
        /// </summary>
        [JsonProperty("size")]
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets the url of the attachment.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the proxied url of the attachment.
        /// </summary>
        [JsonProperty("proxy_url")]
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Gets or sets the height of the attachment.
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the attachment.
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Gets the actual height to render the attachment.
        /// </summary>
        [JsonIgnore]
        public double ActualHeight => Height.HasValue && Width.HasValue ? (double)Height.Value / Width.Value * Math.Min(Width.Value, 400) : double.NaN;

        /// <summary>
        /// Gets the actual width to render the attachment.
        /// </summary>
        [JsonIgnore]
        public double ActualWidth => Width.HasValue ? Math.Min(Width.Value, 400) : double.NaN;

        /// <inheritdoc/>
        [JsonIgnore]
        public string ImageUrl { get => ProxyUrl; }

        /// <inheritdoc/>
        [JsonIgnore]
        public double ImageHeight { get => ActualHeight; }

        /// <inheritdoc/>
        [JsonIgnore]
        public double ImageWidth { get => ActualWidth; }

        /// <inheritdoc/>
        [JsonIgnore]
        public string AnimatedImageUrl { get => null; }
    }
}
