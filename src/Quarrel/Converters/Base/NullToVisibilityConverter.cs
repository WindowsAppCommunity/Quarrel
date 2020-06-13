// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns an <see cref="Visibility.Visible"/> value if the input <see langword="object"/> is <see langword="null"/>.
    /// </summary>
    public sealed class NullToVisibilityConverter
    {
        /// <summary>
        /// Checks if an object is null and returns the alligned visibility.
        /// </summary>
        /// <param name="value">Item to check.</param>
        /// <returns>Alligned visibility to null check.</returns>
        public static Visibility Convert(object value)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Checks if a string is null or empty.
        /// </summary>
        /// <param name="value">String to check.</param>
        /// <returns>Alligned <see cref="Visibility"/> to null or empty check.</returns>
        public static Visibility ConvertString(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
