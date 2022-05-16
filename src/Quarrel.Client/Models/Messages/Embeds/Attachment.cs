// Quarrel © 2022

using Discord.API.Models.Json.Messages.Embeds;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Messages.Embeds
{
    /// <summary>
    /// An attachment in a message.
    /// </summary>
    public class Attachment : SnowflakeItem
    {
        internal Attachment(JsonAttachment jsonAttachment, QuarrelClient context) :
            base(context)
        {
            Id = jsonAttachment.Id;
            Filename = jsonAttachment.Filename;
            Size = jsonAttachment.Size;
            Url = jsonAttachment.Url;
            ProxyUrl = jsonAttachment.ProxyUrl;
            Height = jsonAttachment.Height;
            Width = jsonAttachment.Width;
        }

        /// <summary>
        /// Gets the name of the attached file.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Gets the size of the attached file.
        /// </summary>
        public ulong Size { get; }

        /// <summary>
        /// Gets the url of the attached file.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the proxy url of the attached file.
        /// </summary>
        public string ProxyUrl { get; }

        /// <summary>
        /// Gets the height of the attached file if the file is an image or video.
        /// </summary>
        public int? Height { get; }

        /// <summary>
        /// Gets the width of the attached file if the file is an image or video.
        /// </summary>
        public int? Width { get; }
    }
}
