using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the DateTime type
    /// </summary>
    internal static partial class DateTimeExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static string Humanize(this DateTime value)
        {
            string format = "";

            // Date
            if (DateTime.Now.Date == value.Date)
            {
                format += "Today";
            } else if (DateTime.Now.Date.AddDays(-1) == value.Date)
            {
                format += "Yesterday";
            }else
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
