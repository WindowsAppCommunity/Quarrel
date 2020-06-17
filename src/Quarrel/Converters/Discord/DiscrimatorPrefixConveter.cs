// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that adds a <see langword="'#'"/> to the front of a <see cref="string"/>.
    /// </summary>
    public sealed class DiscrimatorPrefixConveter
    {
        /// <summary>
        /// Adds a <see langword="'#'"/> prefix to a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <returns>The prefixed string</returns>
        public static string Convert(string value)
        {
            return "#" + value.ToString();
        }
    }
}
