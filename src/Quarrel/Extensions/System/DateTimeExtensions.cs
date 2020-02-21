// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the DateTime type.
    /// </summary>
    internal static partial class DateTimeExtensions
    {
        /// <summary>
        /// Formats a <see cref="DateTime"/> in an terms of relative days.
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> to format.</param>
        /// <returns>The Date Time in terms of relative days.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static string Humanize(this DateTime value)
        {
            string format = string.Empty;

            // Date
            if (DateTime.Now.Date == value.Date)
            {
                format += "Today";
            }
            else if (DateTime.Now.Date.AddDays(-1) == value.Date)
            {
                format += "Yesterday";
            }
            else
            {
                format += value.ToString("d");
            }

            format += " at ";

            // Time
            format += value.ToString("t");

            // Finish
            return format;
        }
    }
}
