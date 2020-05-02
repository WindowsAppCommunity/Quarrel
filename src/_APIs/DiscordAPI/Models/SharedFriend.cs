using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models
{
    public class SharedFriend
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        public string avatarUrl(bool useDefault = true, string suffix = "")
        {
            if (String.IsNullOrEmpty(Avatar))
                return useDefault ? "ms-appx:///Assets/DefaultAvatars/QuarrelClassicIcon.png" : null;
            else if (Avatar.StartsWith("a_"))
                return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".gif" + suffix;
            else
                return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".png" + suffix;
        }

        public Uri avatarUri(bool useDefault = true, string suffix = "")
        {
            string url = avatarUrl(useDefault, suffix);
            return url != null ? new Uri(url) : null;
        }

        public string AvatarUrl => avatarUrl();

        public Uri AvatarUri => avatarUri();
    }
}
