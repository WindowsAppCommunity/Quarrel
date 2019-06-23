using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value
    /// </summary>
    public sealed class DiscriminatorToBrushConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
