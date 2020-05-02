// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using QuarrelSmartColor.Extensions.Windows.UI;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter a <see cref="SolidColorBrush"/> from an integer.
    /// </summary>
    public sealed class IntColorToBrushConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush(ColorExtensions.IntToColor((int)value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
