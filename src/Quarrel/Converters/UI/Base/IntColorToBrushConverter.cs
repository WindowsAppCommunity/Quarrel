using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.UI.Base
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value
    /// </summary>
    public sealed class IntColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int color = (int)value;
            if (color == -1)
                return App.Current.Resources["Foreground"];
            else
            {
                if (color != 0)
                {
                    byte a = (byte)(255);
                    byte r = (byte)(color >> 16);
                    byte g = (byte)(color >> 8);
                    byte b = (byte)(color >> 0);
                    return new SolidColorBrush(Color.FromArgb(a, r, g, b));
                }
                else
                {
                    return (SolidColorBrush)App.Current.Resources["Foreground"];
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
