// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a humanized DateTime based on a Discord snowflake (ID).
    /// </summary>
    public class SnowflakeToTimeFormatConveter
    {
        /// <summary>
        /// Converts a snowflake to a humanized date time.
        /// </summary>
        /// <param name="value">The snowflake.</param>
        /// <returns>The date time.</returns>
        public static string Convert(string value)
        {
            return value.AsSnowflakeToTime().LocalDateTime.Humanize();
        }
    }
}
