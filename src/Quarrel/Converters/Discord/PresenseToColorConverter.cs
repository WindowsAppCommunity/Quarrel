// Copyright (c) Quarrel. All rights reserved.

using Windows.UI;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a Color based on a user's presence.
    /// </summary>
    public sealed class PresenseToColorConverter
    {
        /// <summary>
        /// Gets a <see cref="Windows.UI.Color"/> for a presence status.
        /// </summary>
        /// <param name="value">The presence status.</param>
        /// <returns>The appropiate color.</returns>
        public static Color Convert(string value)
        {
            return (Color)App.Current.Resources[(value ?? "offline") + "Color"];
        }
    }
}
