// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns a the default user profile color based on a user's Discriminator.
    /// </summary>
    public sealed class DiscriminatorToBrushConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((int)value % 5)
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

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
