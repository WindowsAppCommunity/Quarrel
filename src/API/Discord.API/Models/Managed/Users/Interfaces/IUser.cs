// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Users;

namespace Discord.API.Models.Users
{
    internal interface IUser : ISnowflakeItem
    {
        string Username { get; }

        int Discriminator { get; }

        string? Avatar { get; }

        string? Bio { get; }

        public string? Banner { get; }

        public string? BannerColor { get; }

        uint? AccentColor { get; }

        bool? Bot { get; }

        UserProperties? Flags { get; }

        UserProperties? PublicFlags { get; }

        string? GetAvatarUrl(uint size);
    }
}
