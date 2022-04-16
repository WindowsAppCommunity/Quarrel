// Quarrel © 2022

using Quarrel.Services.Versioning.Enums;
using Quarrel.Services.Versioning.Models;
using Windows.ApplicationModel;

namespace Quarrel.Services.Versioning
{
    public class VersioningService : IVersioningService
    {
        #if DEV
        private const VersionType ActiveVersionType = VersionType.Dev;
        #elif INSIDER
        private const VersionType ActiveVersionType = VersionType.Insider;
        #elif RELEASE
        private const VersionType ActiveVersionType = VersionType.Release;
        #endif

        public VersioningService()
        {
            AppVersion = new AppVersion()
            {
                MajorVersion = Package.Current.Id.Version.Major,
                MinorVersion = Package.Current.Id.Version.Minor,
                BuildNumber = Package.Current.Id.Version.Build,
                VersionType = ActiveVersionType,
            };

            GitVersionInfo = new GitVersionInfo()
            {
                Branch = ThisAssembly.Git.Branch,
                Commit = ThisAssembly.Git.Commit,
            };
        }

        public AppVersion AppVersion { get; }

        public GitVersionInfo GitVersionInfo { get; }

        public VersionType VersionType => AppVersion.VersionType;
    }
}
