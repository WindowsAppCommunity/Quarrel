// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum representing the content filter level for a guild.
    /// </summary>
    public enum ExplicitContentFilterLevel
    {
        /// <summary>
        /// No explicit content filtering.
        /// </summary>
        Disabled = 0,
        
        /// <summary>
        /// Filter content members without roles.
        /// </summary>
        MembersWithoutRoles = 1,

        /// <summary>
        /// Filters content for all members.
        /// </summary>
        AllMembers = 2
    }
}
