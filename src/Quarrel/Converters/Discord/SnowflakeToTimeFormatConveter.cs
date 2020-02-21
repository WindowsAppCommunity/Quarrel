// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a humanized DateTime based on a Discord snowflake (ID).
    /// </summary>
    public class SnowflakeToTimeFormatConveter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string sValue)
            {
                return sValue.AsSnowflakeToTime().LocalDateTime.Humanize();
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
