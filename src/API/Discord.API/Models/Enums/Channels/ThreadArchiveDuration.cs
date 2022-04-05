// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Channels
{
    /// <summary>
    /// An enum for for duration a thread takes to auto archive with inactivity.
    /// </summary>
    /// <remarks>
    /// Values are in minutes.
    /// </remarks>
    public enum ThreadArchiveDuration
    {
        /// <summary>
        /// The thread will archive with one hour of inactivity.
        /// </summary>
        OneHour = 60,

        /// <summary>
        /// The thread will archive with one day of inactivity.
        /// </summary>
        OneDay = 1440,

        /// <summary>
        /// The thread will archive with three days of inactivity.
        /// </summary>
        ThreeDays = 4320,

        /// <summary>
        /// The thread will archive with one week of inactivity.
        /// </summary>
        OneWeek = 10080
    }
}
