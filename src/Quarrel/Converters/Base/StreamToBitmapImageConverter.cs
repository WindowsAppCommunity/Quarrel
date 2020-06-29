// Copyright (c) Quarrel. All rights reserved.

using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// Converts a <see cref="Stream"/> to a <see cref="BitmapImage"/>.
    /// </summary>
    public class StreamToBitmapImageConverter
    {
        /// <summary>
        /// Converts a stream to a <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">The stream to convert.</param>
        /// <returns>Stream as a <see cref="BitmapImage"/>.</returns>
        public static BitmapImage Convert(object value)
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
    }
}
