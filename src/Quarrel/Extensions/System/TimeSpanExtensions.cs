// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the TimeSpan type.
    /// </summary>
    internal static partial class DateTimeExtensions
    {
        /// <summary>
        /// Puts a <see cref="TimeSpan"/> in a humanized string.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/>.</param>
        /// <param name="ms">Sets if milliseconds should be added.</param>
        /// <returns>A humanized time format string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static string Humanize(this TimeSpan value, bool ms = false)
        {
            string format = @"mm\:ss";
            if (ms)
            {
                format += @"\:ff";
            }

            if (value.Hours > 0)
            {
                format = @"hh\:" + format;
            }

            return value.ToString(format);
        }
    }
}
