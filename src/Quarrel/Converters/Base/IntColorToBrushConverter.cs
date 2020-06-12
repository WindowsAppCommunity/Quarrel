// Copyright (c) Quarrel. All rights reserved.

using QuarrelSmartColor.Extensions.Windows.UI;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter a <see cref="SolidColorBrush"/> from an integer.
    /// </summary>
    public sealed class IntColorToBrushConverter
    {
        /// <summary>
        /// Converts an int to a color.
        /// </summary>
        /// <param name="value">The integer color.</param>
        /// <returns>The <see cref="SolidColorBrush"/> equivelent of the int.</returns>
        public static SolidColorBrush Convert(int value)
        {
            return new SolidColorBrush(ColorExtensions.IntToColor(value));
        }
    }
}
