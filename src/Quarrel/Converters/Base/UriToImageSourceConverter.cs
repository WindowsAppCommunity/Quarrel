// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>.
    /// </summary>
    public sealed class UriToImageSourceConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Uri uri;
            if (value is Uri)
            {
                uri = value as Uri;
                return new BitmapImage(uri);
            }
            else if (value is string sValue)
            {
                if (Uri.TryCreate(sValue, UriKind.Absolute, out uri))
                {
                    return new BitmapImage(uri);
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is BitmapImage image)
            {
                return image.UriSource;
            }

            return null;
        }
    }
}
