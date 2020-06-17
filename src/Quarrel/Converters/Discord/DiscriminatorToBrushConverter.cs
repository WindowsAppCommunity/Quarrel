// Copyright (c) Quarrel. All rights reserved.

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a the default user profile color based on a user's Discriminator.
    /// </summary>
    public sealed class DiscriminatorToBrushConverter
    {
        /// <summary>
        /// Converts a discrimintor to a background brush color.
        /// </summary>
        /// <param name="value">The user's discrimintor</param>
        /// <returns>The backround brush to use.</returns>
        public static SolidColorBrush Convert(int value)
        {
            switch (value % 5)
            {
                case 0: // Blurple
                    return new SolidColorBrush((Color)App.Current.Resources["DiscordBlurple"]);
                case 1: // Grey
                    return new SolidColorBrush((Color)App.Current.Resources["DiscordGray"]);
                case 2: // Green
                    return new SolidColorBrush((Color)App.Current.Resources["DiscordGreen"]);
                case 3: // Yellow
                    return new SolidColorBrush((Color)App.Current.Resources["DiscordYellow"]);
                case 4: // Red
                    return new SolidColorBrush((Color)App.Current.Resources["DiscordRed"]);
            }

            return new SolidColorBrush((Color)App.Current.Resources["DiscordBlurple"]);
        }

        /// <summary>
        /// Converts a discrimintor to a background brush color.
        /// </summary>
        /// <param name="value">The user's discrimintor</param>
        /// <returns>The backround brush to use.</returns>
        public static SolidColorBrush ConvertString(string value)
        {
            return Convert(System.Convert.ToInt32(value));
        }
    }
}
