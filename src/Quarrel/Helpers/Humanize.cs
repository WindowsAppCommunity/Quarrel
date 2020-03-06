// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Helpers
{
    /// <summary>
    /// Contains tools methods for putting values in "humanized" form.
    /// </summary>
    public static class Humanize
    {
        /// <summary>
        /// Converters a byte count to the most appropiate units.
        /// </summary>
        /// <param name="i">Byte count.</param>
        /// <returns><paramref name="i"/> bytes in the most appropiate unit.</returns>
        public static string HumanizeFileSize(long i)
        {
            long absolute_i = i < 0 ? -i : i;
            string suffix;
            double readable;

            if (absolute_i >= 0x40000000)
            {
                // Gigabyte
                suffix = "GB";
                readable = i >> 20;
            }
            else if (absolute_i >= 0x100000)
            {
                // Megabyte
                suffix = "MB";
                readable = i >> 10;
            }
            else if (absolute_i >= 0x400)
            {
                // Kilobyte
                suffix = "KB";
                readable = i;
            }
            else
            {
                // Byte
                return i.ToString("0 B");
            }

            readable = readable / 1024;
            return readable.ToString("0.### ") + suffix;
        }
    }
}
