using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models
{
    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("bot")]
        public bool Bot { get; set; }

        [JsonProperty("mfa_enabled")]
        public bool MfaEnabled { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }


        public Uri AvatarUri => new Uri(AvatarUrl);

        public string AvatarUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Avatar))
                    return "ms-appx:///Assets/DefaultAvatars/QuarrelClassicIcon.png";
                if (Avatar.StartsWith("a_"))
                    return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".gif?size=128";
                return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".png?size=128";
            }
        }
    }
}
