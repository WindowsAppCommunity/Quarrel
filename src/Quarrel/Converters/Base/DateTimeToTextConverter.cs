// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="string"/> form of a <see cref="DateTime"/>.
    /// </summary>
    public class DateTimeToTextConverter
    {
        /// <summary>
        /// Converts a date time to a humanized string form.
        /// </summary>
        /// <param name="dt">The date time to print.</param>
        /// <returns>The date time in string form.</returns>
        public static string Convert(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return dt.Value.Humanize();
            }

            return string.Empty;
        }
    }
}
