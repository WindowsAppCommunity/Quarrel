using System;
using System.IO;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Converters.Base
{
    public class StreamToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Stream stream)
            {
                var bitmapimage = new BitmapImage();
                bitmapimage.SetSource(stream.AsRandomAccessStream().CloneStream());
                return bitmapimage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
