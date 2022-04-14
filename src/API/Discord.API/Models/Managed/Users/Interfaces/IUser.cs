// Quarrel © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Users;

namespace Discord.API.Models.Users
{
    internal interface IUser : ISnowflakeItem
    {
        /// <summary>
        /// Gets the user's username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the user's discriminator.
        /// </summary>
        int Discriminator { get; }

        /// <summary>
        /// Gets the user's avatar id.
        /// </summary>
        string? Avatar { get; }

        /// <summary>
        /// Gets the user's bio.
        /// </summary>
        string? Bio { get; }

        /// <summary>
        /// Gets the user's bio id.
        /// </summary>
        string? Banner { get; }

        /// <summary>
        /// Gets the user's banner color.
        /// </summary>
        string? BannerColor { get; }

        /// <summary>
        /// Gets the user's accent color.
        /// </summary>
        uint? AccentColor { get; }

        /// <summary>
        /// Gets whether or not the user is a bot.
        /// </summary>
        bool? Bot { get; }

        /// <summary>
        /// Gets the user flags.
        /// </summary>
        UserProperties? Flags { get; }

        /// <summary>
        /// Gets the user's public flags.
        /// </summary>
        UserProperties? PublicFlags { get; }

        /// <summary>
        /// Gets the url for an avatar image.
        /// </summary>
        /// <param name="size">The size of the image.</param>
        string? GetAvatarUrl(uint size);
    }
}
