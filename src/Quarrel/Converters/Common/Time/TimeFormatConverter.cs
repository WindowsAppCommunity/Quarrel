// Quarrel © 2022

using System;

namespace Quarrel.Converters.Common.Time
{
    public class TimeFormatConverter
    {
        public static string Convert(DateTimeOffset dateTime)
        {
            return dateTime.ToString("t");
        }
    }
}
