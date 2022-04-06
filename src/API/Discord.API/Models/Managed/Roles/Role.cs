// Adam Dernis © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Enums.Permissions;
using Discord.API.Models.Json.Roles;
using Discord.API.Models.Roles.Interfaces;

namespace Discord.API.Models.Roles
{
    /// <summary>
    /// A role managed by a <see cref="DiscordClient"/>.
    /// </summary>
    internal class Role : SnowflakeItem, IRole
    {
        internal Role(JsonRole jsonRole, DiscordClient context) :
            base(context)
        {
            Id = jsonRole.Id;
            Name = jsonRole.Name;
            Position = jsonRole.Position;
            Icon = jsonRole.Icon;
            Permissions = (Permission)jsonRole.Permissions;
            IsHoisted = jsonRole.Hoist;
            IsMangaged = jsonRole.Managed;
            IsMentionable = jsonRole.Mentionable;
        }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string? Icon { get; private set; }

        /// <inheritdoc/>
        public int Position { get; private set; }

        /// <inheritdoc/>
        public Permission Permissions { get; private set; }

        /// <inheritdoc/>
        public bool IsHoisted { get; private set; }

        /// <inheritdoc/>
        public bool IsMangaged { get; private set; }

        /// <inheritdoc/>
        public bool IsMentionable { get; private set; }
    }
}
