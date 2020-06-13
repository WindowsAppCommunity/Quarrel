// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see langword="true"/> <see cref="bool"/> value indicating if the input <see langword="object"/> is **not** <see langword="null"/>.
    /// </summary>
    public sealed class NotNullToBoolConverter
    {
        /// <summary>
        /// Check if a value is null.
        /// </summary>
        /// <param name="value">Item to check.</param>
        /// <returns>Whether or not item is null.</returns>
        public static bool Convert(object value)
        {
            return value != null;
        }

        /// <summary>
        /// Check if a string is null.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>Whether or not item is null.</returns>
        public static bool Convert(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}