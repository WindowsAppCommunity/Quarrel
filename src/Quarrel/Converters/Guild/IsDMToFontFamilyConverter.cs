// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Guild
{
    /// <summary>
    /// A converter that returns a FontFamily based on if the Guild is a DM.
    /// </summary>
    public sealed class IsDMToFontFamilyConverter
    {
        /// <summary>
        /// Returns the FontFamily to use dependent on if a guild is a DM.
        /// </summary>
        /// <param name="value">Whether or not the guild is a DM.</param>
        /// <returns>The <see cref="FontFamily"/> to use.</returns>
        public static FontFamily Convert(bool value)
        {
            return value ? new FontFamily("Segoe MDL2 Assets") : new FontFamily("Segoe UI");
        }
    }
}
