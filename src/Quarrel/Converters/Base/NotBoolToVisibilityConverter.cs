// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value.
    /// </summary>
    public sealed class NotBoolToVisibilityConverter
    {
        /// <summary>
        /// Inverts a boolean and converts to a <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">The original boolean.</param>
        /// <returns>The inverted boolean as a visibility.</returns>
        public static Visibility Convert(bool value)
        {
            return value ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
