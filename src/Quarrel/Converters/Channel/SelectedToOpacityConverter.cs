using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// A converter than returns <see langword="0.2"/> when <see langword="true"/> or <see langword="0.0"/> when <see langword="false"/>.
    /// </summary>
    public sealed class SelectedToOpacityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 0.2 : 0;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
