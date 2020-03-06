// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// A converter than returns a foreground SolidColorBrush for the channel name based on type.
    /// </summary>
    public sealed class TypeToForegroundBrushConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 4 ? App.Current.Resources["SystemControlBackgroundAccentBrush"] : App.Current.Resources["Foreground"];
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
