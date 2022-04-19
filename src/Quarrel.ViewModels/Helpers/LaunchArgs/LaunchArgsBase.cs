// Quarrel © 2022

using System;
using System.Text.RegularExpressions;

namespace Quarrel.Helpers.LaunchArgs.Models
{
    /// <summary>
    /// The base type for a launch argument class.
    /// </summary>
    public abstract class LaunchArgsBase
    {
        /// <summary>
        /// A regular expression pattern to determine if a launchUri is valid.
        /// </summary>
        /// <remarks>
        /// Group 1 is all segments.
        /// Group 2 is just the first segment of the args.
        /// </remarks>
        private const string BaseRegex = @"^(?:(?:quarrel)|(?:discord)):\/\/(([^/]*)/.*)$";

        /// <summary>
        /// Runs after launch.
        /// </summary>
        public abstract void RunPostLoad(IServiceProvider serviceProvider);

        /// <summary>
        /// Parses a <see cref="LaunchArgsBase"/> from the launch uri.
        /// </summary>
        public static LaunchArgsBase? Parse(string launchUri)
        {
            Match match = Regex.Match(launchUri, BaseRegex);
            if (match.Success)
            {
                switch (match.Groups[2].Value)
                {
                    case "c":
                    case "channel":
                    case "g":
                    case "guild":
                        return NavigateLaunchArgs.Parse(match.Groups[1].Value);
                }
            }

            return null;
        }
    }
}
