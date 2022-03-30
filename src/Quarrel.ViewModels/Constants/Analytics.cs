// Adam Dernis © 2022

namespace Quarrel
{
    /// <summary>
    /// A static class containing constant values for the app.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Contants strings for analytics.
        /// </summary>
        public static class Analytics
        {
            /// <summary>
            /// Constant string for analytics events.
            /// </summary>
            public static class Events
            {
                /// <summary>
                /// The event name for when a user logs in with a token.
                /// </summary>
                public static string LoggedInWithToken => nameof(LoggedInWithToken);
            }
        }
    }
}
