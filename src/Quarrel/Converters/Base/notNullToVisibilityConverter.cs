// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="Visibility.Visible"/> value if the input <see langword="object"/> is <see langword="null"/>.
    /// </summary>
    public sealed class NotNullToVisibilityConverter
    {
        /// <summary>
        /// Checks if a object is null, and returns the inverse visibility.
        /// </summary>
        /// <param name="value">Item to check.</param>
        /// <returns>An inverse visibility for if the object is null.</returns>
        public static Visibility Convert(object value)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Checks if a string is null or empty, and returns the inverse visibility.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>An inverse visibility for if the string is null or empty.</returns>
        public static Visibility Convert(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
