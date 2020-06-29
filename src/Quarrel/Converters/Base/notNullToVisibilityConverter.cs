// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="Visibility.Visible"/> value if the input <see langword="object"/> is <see langword="null"/>.
    /// </summary>
    public sealed class NotNullToVisibilityConverter : IValueConverter
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
        public static Visibility ConvertString(string value)
        {
            return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string sValue)
            {
                return ConvertString(sValue);
            }

            return Convert(value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
