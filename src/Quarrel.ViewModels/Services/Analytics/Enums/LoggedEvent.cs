// Quarrel © 2022

namespace Quarrel.Services.Analytics.Enums
{
    /// <summary>
    /// An enum for event types to log.
    /// </summary>
    public enum LoggedEvent
    {
        /// <summary>
        /// The app logged in successfully.
        /// </summary>
        SuccessfulLogin,

        /// <summary>
        /// The app logged failed to login.
        /// </summary>
        LoginFailed,
    }
}
