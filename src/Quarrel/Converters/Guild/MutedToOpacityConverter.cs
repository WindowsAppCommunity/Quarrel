// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Guild
{
    /// <summary>
    /// A converter that returns the opacity a GuildIcon should appear if it is or is not muted.
    /// </summary>
    public sealed class MutedToOpacityConverter
    {
        /// <summary>
        /// Converts whether or not a guild is muted to the opacity of the guild icon.
        /// </summary>
        /// <param name="value">Whether or not the guild is muted.</param>
        /// <returns>The opacity of the icon.</returns>
        public static double Convert(bool value)
        {
            return value ? 0.5 : 1;
        }
    }
}
