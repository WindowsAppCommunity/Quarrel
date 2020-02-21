// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Guild
{
    /// <summary>
    /// A converter that returns a FontFamily based on if the Guild is a DM.
    /// </summary>
    public sealed class IsDMToFontFamilyConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? new FontFamily("Segoe MDL2 Assets") : new FontFamily("Segoe UI");
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
