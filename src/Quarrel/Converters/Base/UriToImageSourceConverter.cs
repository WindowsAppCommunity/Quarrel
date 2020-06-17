// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>.
    /// </summary>
    public sealed class UriToImageSourceConverter
    {
        /// <summary>
        /// Converts a <see cref="Uri"/> or url string to an <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">The uri or url string.</param>
        /// <returns>A <see cref="BitmapImage"/>.</returns>
        public static BitmapImage Convert(object value)
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
    }
}
