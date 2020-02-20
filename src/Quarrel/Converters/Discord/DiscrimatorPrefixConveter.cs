// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that adds a <see langword="'#'"/> to the front of a <see cref="string"/>.
    /// </summary>
    public sealed class DiscrimatorPrefixConveter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "#" + value.ToString();
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
