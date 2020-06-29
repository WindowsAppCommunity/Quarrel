// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="string"/> form of a <see cref="TimeSpan"/> value.
    /// </summary>
    public sealed class TimeSpanToTextConverter
    {
        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a humanized string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to print.</param>
        /// <returns>The printed <see cref="TimeSpan"/>.</returns>
        public static string Convert(TimeSpan value)
        {
            return value.Humanize();
        }
    }
}
