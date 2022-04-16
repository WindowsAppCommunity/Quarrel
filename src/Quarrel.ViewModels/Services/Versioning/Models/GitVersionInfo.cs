// Quarrel © 2022

namespace Quarrel.Services.Versioning.Models
{
    /// <summary>
    /// A struct containing git version info.
    /// </summary>
    public struct GitVersionInfo
    {
        /// <summary>
        /// The git commit the build was made from.
        /// </summary>
        public string Commit { get; set; }

        /// <summary>
        /// The git branch the build was made from.
        /// </summary>
        public string Branch { get; set; }
    }
}
