// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum represeting the duration for an AFK timeout.
    /// </summary>
    /// <remarks>
    /// Value is by seconds.
    /// </remarks>
    public enum AFKTimeout
    {
        /// <summary>
        /// A user will move to the AFK channel after one minute.
        /// </summary>
        OneMinute = 60,

        /// <summary>
        /// A user will move to the AFK channel after five minutes.
        /// </summary>
        FiveMinutes = 300,

        /// <summary>
        /// A user will move to the AFK channel after fifteen minutes.
        /// </summary>
        FifteenMinutes = 900,

        /// <summary>
        /// A user will move to the AFK channel after thirty minutes.
        /// </summary>
        ThirtyMinutes = 1800,

        /// <summary>
        /// A user will move to the AFK channel after one hour.
        /// </summary>
        OneHour = 3600,
    }
}
