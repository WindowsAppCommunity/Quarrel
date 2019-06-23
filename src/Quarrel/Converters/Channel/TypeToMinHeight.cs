using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeToMinHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)((int)value == 4 ? 48 : 40);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
