namespace DiscordAPI.API.Guild.Models
{
    public class SearchArgs
    {
        public enum HasType
        {
            None,
            Link,
            Embed,
            File,
            Video,
            Image,
            Sound
        };

        public string Content { get; set; }

        public string AuthorId { get; set; }

        public string MentionId { get; set; }

        public HasType Has { get; set; }

        //TODO: Timestamp to ID for

        //Before

        //After

        public string ChannelId { get; set; }

        private string HasToString(HasType type)
        {
            switch (type)
            {
                case HasType.Link:
                    return "link";
                case HasType.Embed:
                    return "embed";
                case HasType.File:
                    return "file";
                case HasType.Video:
                    return "video";
                case HasType.Image:
                    return "image";
                case HasType.Sound:
                    return "sound";
            }
            return "";
        }

        public string ConvertToArgs()
        {
            string args = "?";
            if (Content == null)
            {
                args += Content;
                args += "&";
            }
            if (AuthorId == null)
            {
                args += AuthorId;
                args += "&";
            }
            if (MentionId == null)
            {
                args += MentionId;
                args += "&";
            }
            if (Has == HasType.None)
            {
                args += HasToString(Has);
                args += "&";
            }
            if (ChannelId == null)
            {
                args += ChannelId;
                args += "&";
            }

            args.Remove(args.Length - 2);
            return args;
        }
    }
}
