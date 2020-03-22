// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages
{
    /// <summary>
    /// A converter that returns a humaized filesize based on number of byte.
    /// </summary>
    public sealed class SizeToHumanizedSizeConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Humanize.HumanizeFileSize(System.Convert.ToInt64(value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
