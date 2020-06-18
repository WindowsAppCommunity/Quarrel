// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see langword="true"/> <see cref="bool"/> value indicating if the input <see langword="object"/> is <see langword="null"/>.
    /// </summary>
    public sealed class NullToBoolConverter
    {
        /// <summary>
        /// Checks if a value us null.
        /// </summary>
        /// <param name="value">Item to check.</param>
        /// <returns>Whether or not the item is null.</returns>
        public static bool Convert(object value)
        {
            return value == null;
        }

        /// <summary>
        /// Check if a string is null or empty.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>Whether or not the string is null or empty.</returns>
        public static bool ConvertString(string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
