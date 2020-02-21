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
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool v;
            if (value is string sValue)
            {
                v = !string.IsNullOrEmpty(sValue);
            }
            else
            {
                v = value != null;
            }

            return v;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}