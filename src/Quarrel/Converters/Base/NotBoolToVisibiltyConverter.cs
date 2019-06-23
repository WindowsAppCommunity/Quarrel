using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns an inverted <see cref="Visibility"/> value for the input <see langword="bool"/> value
    /// </summary>
    public sealed class NotBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
