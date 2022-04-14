// Quarrel © 2022

namespace Discord.API.Models.Enums.Users
{
    /// <summary>
    /// The user's online status.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// The user is offline.
        /// </summary>
        Offline,

        /// <summary>
        /// The user is online.
        /// </summary>
        Online,

        /// <summary>
        /// The user is idle.
        /// </summary>
        Idle,

        /// <summary>
        /// The user is AFK.
        /// </summary>
        AFK,

        /// <summary>
        /// The user is on do not disturb.
        /// </summary>
        DoNotDisturb,

        /// <summary>
        /// The user is marked offline.
        /// </summary>
        Invisible,
    }
}
