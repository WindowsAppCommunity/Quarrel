using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Windows.ApplicationModel.Appointments;

namespace System
{
    /// <summary>
    /// A <see langword="class"/> with some extension methods for the TimeSpan type
    /// </summary>
    internal static partial class DateTimeExtensions
    {
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
