// Copyright (c) Quarrel. All rights reserved.

using System;
using System.IO;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// Converts a <see cref="Stream"/> to a <see cref="BitmapImage"/>.
    /// </summary>
    public class StreamToBitmapImageConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Stream stream)
            {
                var bitmapimage = new BitmapImage();

                // Clone as to not reserve
                bitmapimage.SetSource(stream.AsRandomAccessStream().CloneStream());
                return bitmapimage;
            }

            return null;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
