using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Base
{
    public sealed class RemoveNewLineConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string sValue)
            {
                sValue = sValue.Replace('\n', ' ');
                sValue = sValue.Replace('\r', ' ');
                return sValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
