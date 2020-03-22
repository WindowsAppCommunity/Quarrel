// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Media;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.UWP.Common
{
    /// <summary>
    /// Adds extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets a SolidColorBrush from a hex string.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>A <see cref="SolidColorBrush"/>.</returns>
        public static SolidColorBrush GetSolidColorBrush(this string hex)
        {
            hex = hex.Replace("#", string.Empty);

            byte a = 255;
            int index = 0;

            if (hex.Length == 8)
            {
                a = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
                index += 2;
            }

            byte r = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            index += 2;
            byte g = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            index += 2;
            byte b = (byte)Convert.ToUInt32(hex.Substring(index, 2), 16);
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
    }
}