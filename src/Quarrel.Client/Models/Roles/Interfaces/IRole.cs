// Quarrel © 2022

using Quarrel.Client.Models.Base.Interfaces;

namespace Quarrel.Client.Models.Roles.Interfaces
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
        Permissions.Permissions Permissions { get; }

        /// <summary>
        /// Gets whether or not the role is shown separately in the member list.
        /// </summary>
        bool IsHoisted { get; }

        /// <summary>
        /// Gets whether or not the role is managed by integration.
        /// </summary>
        bool IsManaged { get; }

        /// <summary>
        /// Gets whether or not the role mentionable by anyone.
        /// </summary>
        bool IsMentionable { get; }
    }
}
