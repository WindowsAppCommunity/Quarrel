// Quarrel © 2022

using Discord.API.Models.Enums.Permissions;
using Discord.API.Models.Json.Permissions;

namespace Discord.API.Models
{
    /// <summary>
    /// A permission overwrite.
    /// </summary>
    public class PermissionOverwrite
    {
        internal PermissionOverwrite(JsonOverwrite jsonOverwrite)
        {
            Id = jsonOverwrite.Id;
            Type = jsonOverwrite.Type;
            Allow = (Permission)jsonOverwrite.Allow;
            Deny = (Permission)jsonOverwrite.Deny;
        }

        /// <summary>
        /// Gets the id of the role/member to overwrite for.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Gets the type of overwrite.
        /// </summary>
        /// <remarks>
        /// 0 for roles
        /// 1 for members
        /// </remarks>
        public int Type { get; }

        /// <summary>
        /// The permissions added.
        /// </summary>
        public Permissions Allow { get; }

        /// <summary>
        /// The permissions removed.
        /// </summary>
        public Permissions Deny { get; }
    }
}
