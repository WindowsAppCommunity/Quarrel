// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see langword="true"/> <see cref="bool"/> value indicating if the input <see langword="object"/> is **not** <see langword="null"/>.
    /// </summary>
    public sealed class PositiveIntToBoolConverter
    {
        /// <summary>
        /// Check if an int is positive.
        /// </summary>
        /// <param name="value">Int to check..</param>
        /// <returns>Whether or not int is positive.</returns>
        public static bool Convert(int value)
        {
            return value > 0;
        }
    }
}
