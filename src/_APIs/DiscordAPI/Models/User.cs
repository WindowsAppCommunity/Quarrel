using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        

        public string AvatarUrl(bool useDefault = true, string suffix = "")
        {
            if (String.IsNullOrEmpty(Avatar))
                return useDefault ? "ms-appx:///Assets/DefaultAvatars/QuarrelClassicIcon.png" : null;
            else if (Avatar.StartsWith("a_"))
                return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".gif" + suffix;
            else
                return "https://cdn.discordapp.com/avatars/" + Id + "/" + Avatar + ".png" + suffix;
        }

        public Uri AvatarUri(bool useDefault = true, string suffix = "")
        {
            string url = AvatarUrl(useDefault, suffix);
            return url != null ? new Uri(url) : null;
        }
    }
}
