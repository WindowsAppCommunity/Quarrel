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
    public sealed class PresenseToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return App.Current.Resources[(string)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
