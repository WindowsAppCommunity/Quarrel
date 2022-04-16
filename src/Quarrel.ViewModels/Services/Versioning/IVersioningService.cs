// Quarrel © 2022

using Quarrel.Services.Versioning.Models;

namespace Quarrel.Services.Versioning
{
    /// <summary>
    /// An interface for getting App and Git versioning service.
    /// </summary>
    public interface IVersioningService
    {
        /// <summary>
        /// Gets the app version information.
        /// </summary>
        AppVersion AppVersion { get; }

        /// <summary>
        /// Gets the git version information.
        /// </summary>
        GitVersionInfo GitVersionInfo { get; }
    }
}
