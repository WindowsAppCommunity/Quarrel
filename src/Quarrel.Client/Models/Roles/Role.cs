// Quarrel © 2022

using Discord.API.Models.Enums.Permissions;
using Discord.API.Models.Json.Roles;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Roles.Interfaces;

namespace Quarrel.Client.Models.Roles
{
    /// <summary>
    /// A role managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class Role : SnowflakeItem, IRole
    {
        internal Role(JsonRole jsonRole, QuarrelClient context) :
            base(context)
        {
            Id = jsonRole.Id;
            Name = jsonRole.Name;
            Position = jsonRole.Position;
            Icon = jsonRole.Icon;
            Permissions = (Permission)jsonRole.Permissions;
            IsHoisted = jsonRole.Hoist;
            IsManaged = jsonRole.Managed;
            IsMentionable = jsonRole.Mentionable;
        }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public string? Icon { get; private set; }

        /// <inheritdoc/>
        public int Position { get; private set; }

        /// <inheritdoc/>
        public Permissions.Permissions Permissions { get; private set; }

        /// <inheritdoc/>
        public bool IsHoisted { get; private set; }

        /// <inheritdoc/>
        public bool IsManaged { get; private set; }

        /// <inheritdoc/>
        public bool IsMentionable { get; private set; }
    }
}
