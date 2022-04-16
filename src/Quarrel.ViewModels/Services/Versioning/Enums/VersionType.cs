// Quarrel © 2022

namespace Quarrel.Services.Versioning.Enums
{
    /// <summary>
    /// The type of app version.
    /// </summary>
    public enum VersionType
    {
        /// <summary>
        /// The app's version is release.
        /// </summary>
        Release,

        /// <summary>
        /// The app's version is insider.
        /// </summary>
        Insider,

        /// <summary>
        /// The app's version is development.
        /// </summary>
        Dev,

        /// <summary>
        /// The app's version is in beta.
        /// </summary>
        Beta,

        /// <summary>
        /// The app's version is in alpha.
        /// </summary>
        Alpha,
    }
}
