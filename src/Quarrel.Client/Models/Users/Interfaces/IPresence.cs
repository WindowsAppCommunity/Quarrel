// Quarrel © 2022

using Discord.API.Models.Enums.Users;

namespace Quarrel.Client.Models.Users.Interfaces
{
    internal interface IPresence
    {
        /// <summary>
        /// Gets the user's online status.
        /// </summary>
        UserStatus Status { get; }
    }
}
