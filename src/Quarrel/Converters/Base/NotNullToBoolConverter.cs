// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see langword="true"/> <see cref="bool"/> value indicating if the input <see langword="object"/> is **not** <see langword="null"/>.
    /// </summary>
    public sealed class NotNullToBoolConverter : IValueConverter
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
        public static bool ConvertString(string value)
        {
            return !string.IsNullOrEmpty(value);
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