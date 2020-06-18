// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns an inverted <see cref="bool"/>.
    /// </summary>
    public sealed class NotBoolToBoolConverter
    {
        /// <summary>
        /// Inverts a boolean.
        /// </summary>
        /// <param name="value">The original boolean.</param>
        /// <returns>The inverted boolean.</returns>
        public static bool Convert(bool value)
        {
            return !value;
        }
    }
}
