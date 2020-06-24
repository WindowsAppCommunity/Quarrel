// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.Converters.Base
{

    /// <summary>
    /// A converter that returns a <see cref="Visibility"/> value indicating if the input <see langword="int"/> is greater than 0.
    /// </summary>
    public sealed class PositiveIntToVisibilityConverter
    {
        /// <summary>
        /// Check if an int is positive.
        /// </summary>
        /// <param name="value">Int to check..</param>
        /// <returns>Whether or not int is positive.</returns>
        public static Visibility Convert(int value)
        {
            return value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
