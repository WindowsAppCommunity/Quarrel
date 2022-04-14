// Quarrel © 2022

using Discord.API.Models.Enums.Users;

namespace Discord.API.Models.Users.Interfaces
{
    internal interface IPresence
    {
        /// <summary>
        /// Gets the user's online status.
        /// </summary>
        UserStatus Status { get; }
    }
}
