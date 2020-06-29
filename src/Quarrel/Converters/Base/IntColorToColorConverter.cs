// Copyright (c) Quarrel. All rights reserved.

using QuarrelSmartColor.Extensions.Windows.UI;
using Windows.UI;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a Color from an interger.
    /// </summary>
    public sealed class IntColorToColorConverter
    {
        /// <summary>
        /// Converts an int to a <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The int color.</param>
        /// <returns>The <see cref="Color"/> result.</returns>
        public static Color Convert(int value)
        {
            return ColorExtensions.IntToColor(value);
        }
    }
}
