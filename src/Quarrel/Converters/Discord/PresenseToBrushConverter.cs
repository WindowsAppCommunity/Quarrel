using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Quarrel.Services.Settings.Enums;

namespace Quarrel.Converters.Discord
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value
    /// </summary>
    public sealed class PresenseToBrushConverter : IValueConverter
    {
        public bool UseSystemAccentColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string status = (string) value;
            if (UseSystemAccentColor && (status == "offline" || status == "invisible"))
                return new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
            return App.Current.Resources[status];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
