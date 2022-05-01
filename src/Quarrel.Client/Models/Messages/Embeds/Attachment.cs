// Quarrel © 2022

using Discord.API.Models.Json.Messages.Embeds;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Messages.Embeds
{
    public class Attachment : SnowflakeItem
    {
        internal Attachment(JsonAttachment jsonAttachment, QuarrelClient context) :
            base(context)
        {
            Id = jsonAttachment.Id;
            Filename = jsonAttachment.Filename;
            Url = jsonAttachment.Url;
            ProxyUrl = jsonAttachment.ProxyUrl;
            Height = jsonAttachment.Height;
            Width = jsonAttachment.Width;
        }

        public string Filename { get; }

        public ulong Size { get; }

        public string Url { get; }

        public string ProxyUrl { get; }

        public int? Height { get; }

        public int? Width { get; }
    }
}
