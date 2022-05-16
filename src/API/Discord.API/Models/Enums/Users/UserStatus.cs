// Quarrel © 2022

using Quarrel.Attributes;

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
        [StringValue("offline")]
        Offline,

        /// <summary>
        /// The user is online.
        /// </summary>
        [StringValue("online")]
        Online,

        /// <summary>
        /// The user is idle.
        /// </summary>
        [StringValue("idle")]
        Idle,

        /// <summary>
        /// The user is AFK.
        /// </summary>
        [StringValue("afk")]
        AFK,

        /// <summary>
        /// The user is on do not disturb.
        /// </summary>
        [StringValue("dnd")]
        DoNotDisturb,

        /// <summary>
        /// The user is marked offline.
        /// </summary>
        [StringValue("invisible")]
        Invisible,
    }
}
