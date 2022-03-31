// Adam Dernis © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Users;

namespace Discord.API.Models.Users
{
    public class User : SnowflakeItem, IUser
    {
        internal User(JsonUser restUser)
        {
            Username = restUser.Username;
            Discriminator = restUser.Discriminator;
            Avatar = restUser.Avatar;
            Bio = restUser.Bio;
            Banner = restUser.Banner;
            BannerColor = restUser.BannerColor;
            AccentColor = restUser.AccentColor;
            Bot = restUser.Bot;
            Flags = restUser.Flags;
            PublicFlags = restUser.PublicFlags;
        }

        public string Username { get; private set; }

        public string Discriminator { get; private set; }

        public string? Avatar { get; private set; }

        public string? Bio { get; private set; }

        public string? Banner { get; private set; }

        public string? BannerColor { get; private set; }

        public uint? AccentColor { get; private set; }

        public bool? Bot { get; private set; }

        public UserProperties? Flags { get; private set; }

        public UserProperties? PublicFlags { get; private set; }

        public string? GetAvatarUrl(uint size)
        {
            if (Avatar is null)
            {
                return null;
            }

            return $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png?size={size}";
        }

        internal virtual JsonUser ToRestUser()
            => new JsonUser()
            {
                Id = Id,
                Username = Username,
                Discriminator = Discriminator,
                Avatar = Avatar,
                Bio = Bio,
                Banner = Banner,
                BannerColor = BannerColor,
                AccentColor = AccentColor,
                Bot = Bot,
                Flags = Flags,
                PublicFlags = PublicFlags,
            };
    }
}
