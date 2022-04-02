// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Users;

namespace Discord.API.Models.Users
{
    public class User : SnowflakeItem, IUser
    {
        internal User(JsonUser restUser, DiscordClient context) :
            base(context)
        {
            Id = restUser.Id;
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

        public string Username { get; protected set; }

        public string Discriminator { get; protected set; }

        public string? Avatar { get; protected set; }

        public string? Bio { get; protected set; }

        public string? Banner { get; protected set; }

        public string? BannerColor { get; protected set; }

        public uint? AccentColor { get; protected set; }

        public bool? Bot { get; protected set; }

        public UserProperties? Flags { get; protected set; }

        public UserProperties? PublicFlags { get; protected set; }

        public string? GetAvatarUrl(uint size)
        {
            if (Avatar is null)
            {
                return null;
            }

            return $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png?size={size}";
        }

        internal virtual void UpdateFromRestUser(JsonUser jsonUser)
        {
            Guard.IsEqualTo(Id, jsonUser.Id, nameof(Id));

            Username = jsonUser.Username;
            Discriminator = jsonUser.Discriminator;
            Avatar = jsonUser.Avatar ?? Avatar;
            Bio = jsonUser.Bio ?? Bio;
            Banner = jsonUser.Banner ?? Banner;
            BannerColor = jsonUser.BannerColor ?? BannerColor;
            AccentColor = jsonUser.AccentColor ?? AccentColor;
            Bot = jsonUser.Bot ?? Bot;
            Flags = jsonUser.Flags ?? Flags;
            PublicFlags = jsonUser.PublicFlags ?? PublicFlags;
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
