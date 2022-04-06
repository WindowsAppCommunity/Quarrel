// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Permissions;

namespace Discord.API.Models.Roles.Interfaces
{
    internal interface IRole : ISnowflakeItem
    {
        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the role icon.
        /// </summary>
        string? Icon { get; }

        /// <summary>
        /// Gets the position of the role in ordering.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the guild base permissions for the role.
        /// </summary>
        Permission Permissions { get; }

        /// <summary>
        /// Gets whether or not the role is shown seperately in the memberlist.
        /// </summary>
        bool IsHoisted { get; }

        /// <summary>
        /// Gets whether or not the role is managed by integration.
        /// </summary>
        bool IsMangaged { get; }

        /// <summary>
        /// Gets whether or not the role mentionable by anyone.
        /// </summary>
        bool IsMentionable { get; }
    }
}
