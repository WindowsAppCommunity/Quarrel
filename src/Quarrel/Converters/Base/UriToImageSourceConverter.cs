using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A simple converter that converts a given <see cref="Uri"/> to an <see cref="BitmapImage"/>
    /// </summary>
    public sealed class UriToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Uri)
                return new BitmapImage(value as Uri);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is BitmapImage)
                return (value as BitmapImage).UriSource;
            else
                return null;
        }
    }
}
