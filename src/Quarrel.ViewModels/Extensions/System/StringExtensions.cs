// Copyright (c) Quarrel. All rights reserved.

namespace System
{
    /// <summary>
    /// Extensions to the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets a snowflake <see cref="string"/> as a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="snowflake">A <see cref="string"/> in a snowflake form.</param>
        /// <returns>The <paramref name="snowflake"/> as a <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset AsSnowflakeToTime(this string snowflake)
        {
            if (string.IsNullOrEmpty(snowflake))
            {
                return default(DateTimeOffset);
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64((double)((Convert.ToInt64(snowflake) / 4194304) + 1420070400000)));
        }
    }
}
