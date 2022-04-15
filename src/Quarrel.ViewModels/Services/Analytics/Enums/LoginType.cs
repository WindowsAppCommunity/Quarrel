// Quarrel © 2022

namespace Quarrel.Services.Analytics.Enums
{
    /// <summary>
    /// The type of login performed.
    /// </summary>
    public enum LoginType
    {
        /// <summary>
        /// Unspecified login source.
        /// </summary>
        Unspecified,

        /// <summary>
        /// A user has logged in with a new token.
        /// </summary>
        Fresh,

        /// <summary>
        /// A user logged in as part of startup.
        /// </summary>
        StartupLogin,
    }
}
