// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Users;
using Discord.API.Models.Json.Users;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Users.Interfaces;

namespace Quarrel.Client.Models.Users
{
    /// <summary>
    /// A user managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class User : SnowflakeItem, IUser
    {
        internal User(JsonUser restUser, DiscordClient context) :
            base(context)
        {
            Id = restUser.Id;
            Username = restUser.Username;
            Discriminator = int.Parse(restUser.Discriminator);
            Avatar = restUser.Avatar;
            Bio = restUser.Bio;
            Banner = restUser.Banner;
            BannerColor = restUser.BannerColor;
            AccentColor = restUser.AccentColor;
            Bot = restUser.Bot;
            Flags = restUser.Flags;
            PublicFlags = restUser.PublicFlags;
        }

        /// <inheritdoc/>
        public string Username { get; protected set; }

        /// <inheritdoc/>
        public int Discriminator { get; protected set; }

        /// <inheritdoc/>
        public string? Avatar { get; protected set; }

        /// <inheritdoc/>
        public string? Bio { get; protected set; }

        /// <inheritdoc/>
        public string? Banner { get; protected set; }

        /// <inheritdoc/>
        public string? BannerColor { get; protected set; }

        /// <inheritdoc/>
        public uint? AccentColor { get; protected set; }

        /// <inheritdoc/>
        public bool? Bot { get; protected set; }

        /// <inheritdoc/>
        public UserProperties? Flags { get; protected set; }

        /// <inheritdoc/>
        public UserProperties? PublicFlags { get; protected set; }

        /// <inheritdoc/>
        public RelationshipType RelationshipType { get; internal set; }

        /// <inheritdoc/>
        public Presence? Presence { get; internal set; }

        /// <inheritdoc/>
        public string? GetAvatarUrl(uint size = 128)
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
            Discriminator = int.Parse(jsonUser.Discriminator);
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
                Discriminator = Discriminator.ToString(),
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
