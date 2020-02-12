using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static DateTimeOffset AsSnowflakeToTime(this string snowflake)
        {
            if (String.IsNullOrEmpty(snowflake)) return new DateTimeOffset();
            return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64((double)((Convert.ToInt64(snowflake) / (4194304)) + 1420070400000)));
        }
    }
}
