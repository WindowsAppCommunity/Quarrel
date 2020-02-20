// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="DateTime"/> in the most humanly understandable way.
    /// </summary>
    public sealed class DateTimeToTextConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> to a natural language string.
        /// </summary>
        /// <param name="value"><see cref="DateTime"/> to display.</param>
        /// <param name="targetType"><see cref="string"/>.</param>
        /// <param name="parameter">Extra info.</param>
        /// <param name="language">What language the user is using.</param>
        /// <returns>Natural language <see cref="DateTime"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt)
            {
                return dt.Humanize();
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
