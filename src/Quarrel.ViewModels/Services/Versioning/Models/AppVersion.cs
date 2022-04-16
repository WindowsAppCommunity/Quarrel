// Quarrel © 2022

using Quarrel.Services.Versioning.Enums;

namespace Quarrel.Services.Versioning.Models
{
    /// <summary>
    /// A struct containing the app version information.
    /// </summary>
    public struct AppVersion
    {
        /// <summary>
        /// The app's major version.
        /// </summary>
        public ushort MajorVersion { get; set; }

        /// <summary>
        /// The app's minor version.
        /// </summary>
        public ushort MinorVersion { get; set; }

        /// <summary>
        /// The app's build number.
        /// </summary>
        public ushort BuildNumber { get; set; }

        /// <summary>
        /// The app's version type.
        /// </summary>
        public VersionType VersionType { get; set; }
    }
}
